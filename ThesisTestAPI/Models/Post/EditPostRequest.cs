using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Post
{
    public class EditPostRequest :IRequest<(ProblemDetails?, string?)>
    {
        public Guid PostId {  get; set; }
        public string Caption { get; set; } = string.Empty;
    }
}
