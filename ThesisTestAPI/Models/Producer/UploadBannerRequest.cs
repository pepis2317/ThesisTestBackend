using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Producer
{
    public class UploadBannerRequest : IRequest<(ProblemDetails?, string?)>
    {
        public required Guid ProducerId { get; set; }
        public required IFormFile file { get; set; }
    }
}
