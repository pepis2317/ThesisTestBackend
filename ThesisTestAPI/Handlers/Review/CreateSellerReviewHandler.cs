using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Review;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Review
{
    public class CreateSellerReviewHandler : IRequestHandler<CreateSellerReviewRequest, (ProblemDetails?, string?)>
    {
        private readonly ThesisDbContext _db;
        private readonly RatingService _ratingService;
        public CreateSellerReviewHandler(ThesisDbContext db, RatingService ratingService)
        {
            _db = db;
            _ratingService = ratingService;
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
            await _ratingService.CreateRating(new Models.Rating.CreateRatingRequest
            {
                AuthorId = request.AuthorId,
                Rating = request.Rating,
                ContentId = contentId,
            });
            return (null, contentId.ToString());
        }
    }
}
