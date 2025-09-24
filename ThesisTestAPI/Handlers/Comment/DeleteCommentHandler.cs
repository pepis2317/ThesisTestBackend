using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Comment;

namespace ThesisTestAPI.Handlers.Comment
{
    public class DeleteCommentHandler : IRequestHandler<DeleteCommentRequest, (ProblemDetails?, string?)>
    {
        private readonly ThesisDbContext _db;
        public DeleteCommentHandler(ThesisDbContext db)
        {
            _db = db;
        }
        public async Task<(ProblemDetails?, string?)> Handle(DeleteCommentRequest request, CancellationToken cancellationToken)
        {
            await _db.Contents.Where(c => c.ContentId == request.CommentId).ExecuteDeleteAsync();
            return (null, "Successfully deleted comment");
        }
    }
}
