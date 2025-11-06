using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Enum;
using ThesisTestAPI.Models.MessageAttachments;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.MessageAttachments
{
    public class GetMessageAttachmentsHandler : IRequestHandler<GetMessageAttachmentsRequest, (ProblemDetails?, List<AttachmentDTO>?)>
    {
        private readonly ThesisDbContext _db;
        private readonly BlobStorageService _blobStorageService;
        public GetMessageAttachmentsHandler(ThesisDbContext db, BlobStorageService blobStorageService)
        {
            _db = db;
            _blobStorageService = blobStorageService;
        }

        public async Task<(ProblemDetails?, List<AttachmentDTO>?)> Handle(GetMessageAttachmentsRequest request, CancellationToken cancellationToken)
        {
            var attachments = await _db.MessageAttachments.Where(q => q.MessageId == request.MessageId).ToListAsync();
            var list = new List<AttachmentDTO>();
            foreach (var attachment in attachments)
            {
                var sas = await _blobStorageService.GenerateSasUriAsync(
                     attachmentId: attachment.AttachmentId,
                     blobName: attachment.BlobFileName,
                     fileName: attachment.FileName,
                     mimeType: attachment.FileType,
                     containerName: BlobContainers.MESSAGEATTACHMENTS
                 );
                list.Add(sas);
            }
            return (null, list);
        }
    }
}
