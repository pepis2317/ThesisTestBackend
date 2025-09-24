using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Comment
{
    public class CreateCommentRequest : IRequest<(ProblemDetails?, CommentResponse?)>
    {
        public Guid TargetContentId { get; set; }
        public Guid AuthorId { get;set; }
        public string Comment { get; set; } = string.Empty;

    }
}
