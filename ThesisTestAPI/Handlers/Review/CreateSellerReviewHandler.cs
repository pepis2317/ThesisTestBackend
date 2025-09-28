using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Review;

namespace ThesisTestAPI.Handlers.Review
{
    public class CreateSellerReviewHandler : IRequestHandler<CreateSellerReviewRequest, (ProblemDetails?, string?)>
    {
        private readonly ThesisDbContext _db;
        public CreateSellerReviewHandler(ThesisDbContext db)
        {
            _db = db;
        }
        public async Task<(ProblemDetails?, string?)> Handle(CreateSellerReviewRequest request, CancellationToken cancellationToken)
        {
            var contentId = Guid.NewGuid();
            var content = new Content
            {
                ContentId = contentId,
                AuthorId = request.AuthorId,
                CreatedAt = DateTimeOffset.UtcNow,
            };
            var review = new SellerReview
            {
                SellerReviewId = contentId,
                SellerReviewNavigation = content,
                Review = request.Review,
                SellerId = request.SellerId
            };
            _db.SellerReviews.Add(review);
            await _db.SaveChangesAsync();
            return (null, contentId.ToString());
        }
    }
}
