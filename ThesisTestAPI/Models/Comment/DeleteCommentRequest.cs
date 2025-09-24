using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Comment
{
    public class DeleteCommentRequest:IRequest<(ProblemDetails?, string?)>
    {
        public Guid CommentId { get; set; }
    }
}
