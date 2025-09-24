using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.User
{
    public class UploadPfpRequest : IRequest<(ProblemDetails?, string?)>
    {
        public required Guid UserId { get; set; }
        public required IFormFile File { get; set; }
    }
}
