using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Chat;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Chat
{
    public class SendMessageHandler : IRequestHandler<SendMessageRequest, (ProblemDetails?, MessageResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly IHubContext<ChatHub> _hub;
        private readonly NotificationService _notificationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly MessageAttachmentService _attachmentService;
        public SendMessageHandler(ThesisDbContext db, NotificationService notificationService,MessageAttachmentService attachmentService, IHubContext<ChatHub> hub, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _hub = hub;
            _notificationService = notificationService;
            _httpContextAccessor = httpContextAccessor;
            _attachmentService = attachmentService;
        }
        public async Task<(ProblemDetails?, MessageResponse?)> Handle(SendMessageRequest request, CancellationToken cancellationToken)
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
            if(request.Files!= null && request.Files.Any())
            {
                await _attachmentService.CreateMessageAttachments(new Models.MessageAttachments.CreateMessageAttachmentRequest { MessageId = msg.MessageId,Files = request.Files });
            }
            
            var payload = new MessageResponse()
            {
                MessageId = msg.MessageId,
                Message = msg.Message1,
                SenderId = msg.SenderId,
                CreatedAt = msg.CreatedAt,
                UpdatedAt = msg.UpdatedAt,
                DeletedAt = msg.DeletedAt,
                HasAttachments = msg.HasAttachments
            };
            
            await _db.SaveChangesAsync();
            await _hub.Clients.Group(ChatHub.GroupName(request.ConversationId)).SendAsync("MessageCreated", payload);
            var sender = await _db.Users.Where(q=>q.UserId == request.SenderId).FirstOrDefaultAsync();
            var conversationMembers = await _db.ConversationMembers.Where(q => q.ConversationId == request.ConversationId && q.UserId != request.SenderId).ToListAsync();

            foreach (var member in conversationMembers)
            {
                await _notificationService.SendNotification($"{sender.UserName} sent a new message", member.UserId);
            }

            return (null, payload);
        }
    }
}
