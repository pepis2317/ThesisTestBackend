using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.MessageAttachments
{
    public class CreateMessageAttachmentRequest : IRequest<(ProblemDetails?, string?)>
    {
        public required Guid MessageId { get; set; }
        public required IFormFile File { get; set; }
    }
}
