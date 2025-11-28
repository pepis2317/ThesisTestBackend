using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Review;

namespace ThesisTestAPI.Handlers.Review
{
    public class EditUserReviewHandler : IRequestHandler<EditUserReviewRequest, (ProblemDetails?, string?)>
    {
        private readonly ThesisDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public EditUserReviewHandler(ThesisDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<(ProblemDetails?, string?)> Handle(EditUserReviewRequest request, CancellationToken cancellationToken)
        {
            var review = await _db.UserReviews.Include(q=>q.UserReviewNavigation).Where(q => q.UserReviewId == request.ReviewId).FirstOrDefaultAsync();
            if (review == null)
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
            var rating = await _db.Ratings.Include(q => q.RatingNavigation).Where(q => q.RatingNavigation.ContentId == review.UserReviewId).FirstOrDefaultAsync();
            review.Review = request.Review;
            rating.Rating1 = request.Rating;
            review.UserReviewNavigation.UpdatedAt = DateTimeOffset.Now;
            _db.UserReviews.Update(review);
            await _db.SaveChangesAsync();
            return (null, $@"Successfully Updated {review.UserReviewId}");
        }
    }
}
