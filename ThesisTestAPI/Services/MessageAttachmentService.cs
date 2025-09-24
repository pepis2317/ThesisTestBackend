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
        public async Task<string> CreateMessageAttachment(CreateMessageAttachmentRequest request)
        {
            var contentType = request.File.ContentType;
            using var stream = request.File.OpenReadStream();
            var blobFileName = await _blobStorageService.UploadFileAsync(stream, request.File.FileName, contentType, Enum.BlobContainers.MESSAGEATTACHMENTS);
            var attachment = new MessageAttachment
            {
                AttachmentId = Guid.NewGuid(),
                FileType = request.File.ContentType,
                CreatedAt = DateTimeOffset.Now,
                BlobFileName = blobFileName,
                FileName = request.File.FileName,
                MessageId = request.MessageId,
            };
            _db.MessageAttachments.Add(attachment);
            await _db.SaveChangesAsync();
            return blobFileName;
        }
    }
}
