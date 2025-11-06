using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.MessageAttachments;

namespace ThesisTestAPI.Services
{
    public class MessageAttachmentService
    {
        private readonly ThesisDbContext _db;
        private readonly BlobStorageService _blobStorageService;
        public MessageAttachmentService(ThesisDbContext db, BlobStorageService blobStorageService)
        {
            _db = db;
            _blobStorageService = blobStorageService;
        }
        public async Task<List<MessageAttachmentResponse>> CreateMessageAttachments(CreateMessageAttachmentRequest request)
        {
            var created = new List<MessageAttachment>();
            var list = new  List<MessageAttachmentResponse>();
            foreach (var file in request.Files.Where(q => q.Length > 0)) {
                var contentType = file.ContentType;
                using var stream = file.OpenReadStream();
                var blobFileName = await _blobStorageService.UploadFileAsync(stream, file.FileName, contentType, Enum.BlobContainers.MESSAGEATTACHMENTS);
                var attachment = new MessageAttachment
                {
                    AttachmentId = Guid.NewGuid(),
                    FileType = file.ContentType,
                    CreatedAt = DateTimeOffset.Now,
                    BlobFileName = blobFileName,
                    FileName = file.FileName,
                    MessageId = request.MessageId,
                };
                created.Add(attachment);
                list.Add(new MessageAttachmentResponse
                {
                    AttachmentId = attachment.AttachmentId,
                    ContentType = contentType,
                    BlobFileName = file.FileName,
                    FileName = file.FileName,
                    Size = file.Length
                });
            }
            _db.MessageAttachments.AddRange(created);
            await _db.SaveChangesAsync();
            return list;
        }
    }
}
