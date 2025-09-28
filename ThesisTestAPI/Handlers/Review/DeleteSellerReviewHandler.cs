using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Review;

namespace ThesisTestAPI.Handlers.Review
{
    public class DeleteSellerReviewHandler : IRequestHandler<DeleteSellerReviewRequest, (ProblemDetails?, string?)>
    {
        private readonly ThesisDbContext _db;
        public DeleteSellerReviewHandler(ThesisDbContext db)
        {
            _db = db;
        }
        public async Task<(ProblemDetails?, string?)> Handle(DeleteSellerReviewRequest request, CancellationToken cancellationToken)
        {
            await _db.Contents.Where(q => q.ContentId == request.ReviewId).ExecuteDeleteAsync();
            return (null, "Successfully deleted review");
        }
    }
}
