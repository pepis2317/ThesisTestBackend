using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Producer
{
    public class BannerRequest : IRequest<(ProblemDetails?, string?)>
    {
        public required Guid ProducerId { get; set; }
        public required IFormFile File { get; set; }
    }
}
