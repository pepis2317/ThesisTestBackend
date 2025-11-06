using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Chat;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Chat
{
    public class EditMessageHandler : IRequestHandler<EditMessageRequest, (ProblemDetails?, MessageResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly IHubContext<ChatHub> _hub;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public EditMessageHandler(ThesisDbContext db, IHubContext<ChatHub> hub, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _hub = hub;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<(ProblemDetails?, MessageResponse?)> Handle(EditMessageRequest request, CancellationToken cancellationToken)
        {
            var message = await _db.Messages.FirstOrDefaultAsync(q => q.MessageId == request.MessageId);
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
                HasAttachments = message.HasAttachments,
            };
            await _hub.Clients.Group(ChatHub.GroupName(message.ConversationId)).SendAsync("MessageEdited", payload);
            return (null, payload);
        }
    }
}
