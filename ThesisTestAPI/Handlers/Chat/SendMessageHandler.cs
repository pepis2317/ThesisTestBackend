using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Chat;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Chat
{
    public class SendMessageHandler : IRequestHandler<SendMessageRequest, (ProblemDetails?, string?)>
    {
        private readonly ThesisDbContext _db;
        private readonly IHubContext<ChatHub> _hub;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SendMessageHandler(ThesisDbContext db, IHubContext<ChatHub> hub, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _hub = hub;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<(ProblemDetails?, string?)> Handle(SendMessageRequest request, CancellationToken cancellationToken)
        {
            var isMember = await _db.ConversationMembers.AnyAsync(cm => cm.ConversationId == request.ConversationId && cm.UserId == request.SenderId);
            if (!isMember)
            {
                var problemDetails = new ProblemDetails
                {
                    Type = "http://veryCoolAPI.com/errors/invalid-data",
                    Title = "Invalid Request Data",
                    Detail = "User is not member of this conversation",
                    Instance = _httpContextAccessor.HttpContext?.Request.Path
                };
                return (problemDetails, null);
            }

            var msg = new Message
            {
                MessageId = Guid.NewGuid(),
                ConversationId = request.ConversationId,
                SenderId = request.SenderId,
                Message1 = request.Text ?? "",
                CreatedAt = DateTimeOffset.UtcNow
            };

            _db.Messages.Add(msg);
            await _db.SaveChangesAsync();

            var payload = new
            {
                msg.MessageId,
                request.ConversationId,
                msg.SenderId,
                Message = msg.Message1,
                msg.CreatedAt
            };

            await _hub.Clients.Group($"conv:{request.ConversationId}").SendAsync("MessageCreated", payload);
            return (null, "Message sent");
        }
    }
}
