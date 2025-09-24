using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThesisTestAPI.Entities;

namespace ThesisTestAPI.Models.Comment
{
    public class EditCommentRequest : IRequest<(ProblemDetails?, string?)>
    {
        public Guid CommentId { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
