using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Review;

namespace ThesisTestAPI.Handlers.Review
{
    public class DeleteUserReviewHandler : IRequestHandler<DeleteUserReviewRequest, (ProblemDetails?, string?)>
    {
        private readonly ThesisDbContext _db;
        public DeleteUserReviewHandler(ThesisDbContext db)
        {
            _db = db;
        }
        public async Task<(ProblemDetails?, string?)> Handle(DeleteUserReviewRequest request, CancellationToken cancellationToken)
        {
            await _db.Contents.Where(q => q.ContentId == request.ReviewId).ExecuteDeleteAsync();
            return (null, "Successfully deleted review");
        }
    }
}
