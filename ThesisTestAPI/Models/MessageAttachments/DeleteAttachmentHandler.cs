using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Chat;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Models.MessageAttachments
{
    public class DeleteAttachmentHandler:IRequestHandler<DeleteAttachmentRequest, (ProblemDetails?,bool?)>
    {
        private readonly ThesisDbContext _db;
        private readonly BlobStorageService _blobStorageService;
        private readonly IHubContext<ChatHub> _hub;
        public DeleteAttachmentHandler(ThesisDbContext db, BlobStorageService blobStorageService, IHubContext<ChatHub> hub)
        {
            _db = db;
            _blobStorageService = blobStorageService;
            _hub = hub;
        }

        public async Task<(ProblemDetails?, bool?)> Handle(DeleteAttachmentRequest request, CancellationToken cancellationToken)
        {
            var attachment = await _db.MessageAttachments.Include(q=>q.Message).Where(q => q.AttachmentId == request.AttachmentId).FirstOrDefaultAsync();
            if (attachment == null)
            {
                return (null, false);
            }
            await _blobStorageService.DeleteFileAsync(attachment.BlobFileName, Enum.BlobContainers.MESSAGEATTACHMENTS);
            var message = attachment.Message;
            if (string.IsNullOrEmpty(attachment.Message.Message1))
            {
                message.Message1 = "Message has been deleted";
                message.DeletedAt = DateTimeOffset.Now;
                message.HasAttachments = false;
                _db.Messages.Update(attachment.Message);
            }
            await _db.SaveChangesAsync();
            var payload = new MessageResponse()
            {
                MessageId = message.MessageId,
                Message = message.Message1,
                SenderId = message.SenderId,
                CreatedAt = message.CreatedAt,
                UpdatedAt = message.UpdatedAt,
                DeletedAt = message.DeletedAt,
                Attachments = null
            };
            var attachmentPayload = new AttachmentPayload
            {
                AttachmentId = request.AttachmentId,
                MessageId = message.MessageId
            };
            await _hub.Clients.Group(ChatHub.GroupName(message.ConversationId)).SendAsync("MessageDeleted", payload);
            await _hub.Clients.Group(ChatHub.GroupName(message.ConversationId)).SendAsync("AttachmentDeleted", attachmentPayload);
            return (null, true);
        }
        public class AttachmentPayload
        {
            public Guid AttachmentId {  get; set; }
            public Guid MessageId { get; set; }
        }
    }
}
