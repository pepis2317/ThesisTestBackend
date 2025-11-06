using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.MessageAttachments
{
    public class GetMessageAttachmentsRequest:IRequest<(ProblemDetails?, List<AttachmentDTO>)>
    {
        public Guid MessageId {  get; set; }
    }
    public class AttachmentDTO
    {
        public Guid AttachmentId { get; set; }
        public string FileName { get; set; } = default!;
        public string MimeType { get; set; } = default!;
        public long SizeBytes { get; set; }
        // A short-lived, directly downloadable URL (S3 presigned or Azure SAS)
        public string DownloadUrl { get; set; } = default!;
        public DateTimeOffset ExpiresAt { get; set; }
    }
}
