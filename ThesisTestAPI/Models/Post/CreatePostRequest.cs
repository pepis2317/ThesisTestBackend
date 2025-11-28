using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Post
{
    public class CreatePostRequest : IRequest<(ProblemDetails?, Guid?)>
    {
        public Guid AuthorId { get; set; }
        public string Caption { get;set; } = string.Empty;
        public List<IFormFile> Files { get; set; } = new();
    }
}
