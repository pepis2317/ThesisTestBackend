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
    public class ConversationNotif
    {
        public Guid ConversationId { get; set; }
        public Guid SenderId {  get; set; }
        public string Message { get; set; } = string.Empty;
    }
    public class SendMessageHandler : IRequestHandler<SendMessageRequest, (ProblemDetails?, MessageResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly IHubContext<ChatHub> _hub;
        private readonly NotificationService _notificationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly MessageAttachmentService _attachmentService;
        private readonly BlobStorageService _blobStorageService;
        public SendMessageHandler(ThesisDbContext db, BlobStorageService blobStorageService, NotificationService notificationService,MessageAttachmentService attachmentService, IHubContext<ChatHub> hub, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _hub = hub;
            _notificationService = notificationService;
            _httpContextAccessor = httpContextAccessor;
            _attachmentService = attachmentService;
            _blobStorageService = blobStorageService;
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
            var hasAttachments = request.Files != null && request.Files.Any();
            var messageId = Guid.NewGuid();
            if (request.MessageId != null)
            {
                messageId = (Guid) request.MessageId;
            }
            var msg = new Message
            {
                MessageId = messageId,
                ConversationId = request.ConversationId,
                SenderId = request.SenderId,
                Message1 = request.Text ?? "",
                CreatedAt = DateTimeOffset.UtcNow,
                HasAttachments = hasAttachments
            };

            _db.Messages.Add(msg);
            if(hasAttachments)
            {
                await _attachmentService.CreateMessageAttachments(new Models.MessageAttachments.CreateMessageAttachmentRequest { MessageId = msg.MessageId,Files = request.Files });
            }
            var conversation = await _db.Conversations.Where(q => q.ConversationId == request.ConversationId).FirstOrDefaultAsync();
            var attachments = await _db.MessageAttachments.Where(q => q.MessageId == msg.MessageId).OrderByDescending(q=>q.CreatedAt).ToListAsync();
            conversation.UpdatedAt = DateTimeOffset.Now;
            var payload = new MessageResponse()
            {
                MessageId = msg.MessageId,
                Message = msg.Message1,
                SenderId = msg.SenderId,
                CreatedAt = msg.CreatedAt,
                UpdatedAt = msg.UpdatedAt,
                DeletedAt = msg.DeletedAt,
                Sent = true
            };
            if (hasAttachments)
            {
                var attachmentDtos = new List<AttachmentDTO>();
                foreach(var attachment in attachments)
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

            var notif = new ConversationNotif()
            {
                ConversationId = msg.ConversationId,
                Message = msg.Message1,
                SenderId = msg.SenderId,
            };

            
            await _db.SaveChangesAsync();
            await _hub.Clients.Group(ChatHub.GroupName(request.ConversationId)).SendAsync("MessageCreated", payload);
            var sender = await _db.Users.Where(q=>q.UserId == request.SenderId).FirstOrDefaultAsync();
            var conversationMembers = await _db.ConversationMembers.Where(q => q.ConversationId == request.ConversationId && q.UserId != request.SenderId).ToListAsync();

            foreach (var member in conversationMembers)
            {
                await _hub.Clients.Group(member.UserId.ToString()).SendAsync("NewMessage", notif);
                await _notificationService.SendNotification($"{sender.UserName} sent a new message", member.UserId);
            }

            return (null, payload);
        }
    }
}
