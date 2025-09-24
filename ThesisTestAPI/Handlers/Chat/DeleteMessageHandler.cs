using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Chat;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Chat
{
    public class DeleteMessageHandler : IRequestHandler<DeleteMessageRequest, (ProblemDetails?, string?)>
    {
        private readonly ThesisDbContext _db;
        private readonly BlobStorageService _blobStorageService;
        private readonly IHubContext<ChatHub> _hub;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DeleteMessageHandler(ThesisDbContext db, IHubContext<ChatHub> hub, IHttpContextAccessor httpContextAccessor, BlobStorageService blobStorageService)
        {
            _db = db;
            _hub = hub;
            _httpContextAccessor = httpContextAccessor;
            _blobStorageService = blobStorageService;
        }
        public async Task<(ProblemDetails?, string?)> Handle(DeleteMessageRequest request, CancellationToken cancellationToken)
        {
            var message = await _db.Messages.FirstOrDefaultAsync(q => q.MessageId == request.MessageId && q.ConversationId == request.ConversationId);
            if (message == null)
            {
                var problemDetails = new ProblemDetails
                {
                    Type = "http://veryCoolAPI.com/errors/invalid-data",
                    Title = "Invalid Request Data",
                    Detail = "No message with such id was found",
                    Instance = _httpContextAccessor.HttpContext?.Request.Path
                };
                return (problemDetails, null);
            }
            var attachments = await _db.MessageAttachments.Where(q=>q.MessageId == request.MessageId).ToListAsync();
            foreach(var attachment in attachments)
            {
                await _blobStorageService.DeleteFileAsync(attachment.BlobFileName, Enum.BlobContainers.MESSAGEATTACHMENTS);
            }
            message.Message1 = "";
            message.DeletedAt = DateTimeOffset.Now;
            _db.Messages.Update(message);
            await _db.SaveChangesAsync();
            var payload = new
            {
                message.MessageId,
                request.ConversationId,
                message.SenderId,
                Message = message.Message1,
                message.DeletedAt
            };
            await _hub.Clients.Group($"conv:{request.ConversationId}").SendAsync("MessageDeleted", payload);
            return (null, "Message deleted");
        }
    }
}
