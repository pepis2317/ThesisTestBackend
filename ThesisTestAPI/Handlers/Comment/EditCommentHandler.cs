using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Comment;
using ThesisTestAPI.Models.Post;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace ThesisTestAPI.Handlers.Comment
{
    public class EditCommentHandler : IRequestHandler<EditCommentRequest, (ProblemDetails?, string?)>
    {
        private readonly ThesisDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public EditCommentHandler(ThesisDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<(ProblemDetails?, string?)> Handle(EditCommentRequest request, CancellationToken cancellationToken)
        {
            var comment = await _db.Comments.Where(q => q.CommentId == request.CommentId).FirstOrDefaultAsync();
            if (comment != null)
            {
                comment.Comment1 = request.Comment;
                comment.CommentNavigation.UpdatedAt = DateTimeOffset.Now;
                _db.Comments.Update(comment);
                await _db.SaveChangesAsync();
                return (null, "Successfully edited comment");
            }
            var problemDetails = new ProblemDetails
            {
                Type = "http://veryCoolAPI.com/errors/invalid-data",
                Title = "Invalid Request Data",
                Detail = "No comment with such id was found",
                Instance = _httpContextAccessor.HttpContext?.Request.Path
            };
            return (problemDetails, null);

        }
    }
}
