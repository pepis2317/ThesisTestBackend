using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Producer
{
    public class UploadBannerRequest : IRequest<(ProblemDetails?, string?)>
    {
        public required Guid SellerId { get; set; }
        public required IFormFile file { get; set; }
    }
}
