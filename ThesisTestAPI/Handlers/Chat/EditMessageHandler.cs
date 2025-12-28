using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Enum;
using ThesisTestAPI.Models.Chat;
using ThesisTestAPI.Models.MessageAttachments;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Chat
{
    public class EditMessageHandler : IRequestHandler<EditMessageRequest, (ProblemDetails?, MessageResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly BlobStorageService _blobStorageService;
        private readonly IHubContext<ChatHub> _hub;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public EditMessageHandler(ThesisDbContext db, IHubContext<ChatHub> hub, BlobStorageService blobStorageService, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _hub = hub;
            _blobStorageService = blobStorageService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<(ProblemDetails?, MessageResponse?)> Handle(EditMessageRequest request, CancellationToken cancellationToken)
        {
            var message = await _db.Messages.Include(q=>q.MessageAttachments).FirstOrDefaultAsync(q => q.MessageId == request.MessageId);
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
            message.Message1 = request.Text;
            message.UpdatedAt = DateTimeOffset.Now;
            _db.Messages.Update(message);
            await _db.SaveChangesAsync();
            var payload = new MessageResponse()
            {
                MessageId = message.MessageId,
                Message = message.Message1,
                SenderId = message.SenderId,
                CreatedAt = message.CreatedAt,
                UpdatedAt = message.UpdatedAt,
                DeletedAt = message.DeletedAt,
                Sent = true
            };
            if (message.MessageAttachments.Any())
            {
                var attachmentDtos = new List<AttachmentDTO>();
                foreach( var attachment in message.MessageAttachments)
                {
                    var sas = await _blobStorageService.GenerateSasUriAsync(
                        attachmentId: attachment.AttachmentId,
                        blobName: attachment.BlobFileName,
                        fileName: attachment.FileName,
                        mimeType: attachment.FileType,
                        containerName: BlobContainers.MESSAGEATTACHMENTS
                    );
                    attachmentDtos.Add(sas);
                }
                payload.Attachments = attachmentDtos;
            }
            await _hub.Clients.Group(ChatHub.GroupName(message.ConversationId)).SendAsync("MessageEdited", payload);
            return (null, payload);
        }
    }
}
