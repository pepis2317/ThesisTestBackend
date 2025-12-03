using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.MessageAttachments
{
    public class DeleteAttachmentRequest:IRequest<(ProblemDetails?, bool?)>
    {
        public Guid AttachmentId { get; set; }
    }
}
