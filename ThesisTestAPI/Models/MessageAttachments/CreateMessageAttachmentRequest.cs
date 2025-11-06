using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.MessageAttachments
{
    public class CreateMessageAttachmentRequest : IRequest<(ProblemDetails?, List<MessageAttachmentResponse>?)>
    {
        public required Guid MessageId { get; set; }
        public required List<IFormFile> Files { get; set; } = new();
    }
}
