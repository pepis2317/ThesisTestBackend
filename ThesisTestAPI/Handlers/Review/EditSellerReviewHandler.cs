using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Seller;
using ThesisTestAPI.Models.Review;

namespace ThesisTestAPI.Handlers.Review
{
    public class EditSellerReviewHandler : IRequestHandler<EditSellerReviewRequest, (ProblemDetails?, string?)>
    {
        private readonly ThesisDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public EditSellerReviewHandler(ThesisDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<(ProblemDetails?, string?)> Handle(EditSellerReviewRequest request, CancellationToken cancellationToken)
        {
            var review = await _db.SellerReviews.Include(q=>q.SellerReviewNavigation).Where(q => q.SellerReviewId == request.ReviewId).FirstOrDefaultAsync();
            if(review == null)
            {
                var problemDetails = new ProblemDetails
                {
                    Type = "http://veryCoolAPI.com/errors/invalid-data",
                    Title = "Invalid Request Data",
                    Detail = "No review with such id was found",
                    Instance = _httpContextAccessor.HttpContext?.Request.Path
                };
                return (problemDetails, null);
            }
            var rating = await _db.Ratings.Include(q=>q.RatingNavigation).Where(q=>q.RatingNavigation.ContentId == review.SellerReviewId).FirstOrDefaultAsync();
            review.Review = request.Review;
            rating.Rating1 = request.Rating;
            review.SellerReviewNavigation.UpdatedAt = DateTimeOffset.Now;
            _db.SellerReviews.Update(review);
            await _db.SaveChangesAsync();
            return (null, $@"Successfully Updated {review.SellerReviewId}");
        }
    }
}
