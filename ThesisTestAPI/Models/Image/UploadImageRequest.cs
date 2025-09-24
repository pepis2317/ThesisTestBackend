using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Image
{
    public class UploadImageRequest : IRequest<(ProblemDetails?, string? )>
    {
        public required Guid ContentId { get; set; }    
        public required IFormFile File { get; set; }
    }
}
