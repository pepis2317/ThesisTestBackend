using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Review;

namespace ThesisTestAPI.Handlers.Review
{
    public class GetUserStatsHandler : IRequestHandler<GetUserStatsRequest, (ProblemDetails?, UserStatsResponse?)>
    {
        private readonly ThesisDbContext _db;
        public GetUserStatsHandler(ThesisDbContext db)
        {
            _db = db;
        }

        public async Task<(ProblemDetails?, UserStatsResponse?)> Handle(GetUserStatsRequest request, CancellationToken cancellationToken)
        {
            var reviewIds = await _db.UserReviews.Where(q => q.UserId == request.UserId).Select(q => q.UserReviewId).ToListAsync();
            var ratings = await _db.Ratings.Include(q => q.RatingNavigation).Where(q => reviewIds.Contains(q.RatingNavigation.ContentId)).Select(q => q.Rating1).AverageAsync();
            return (null, new UserStatsResponse
            {
                Rating = ratings != null ? (double)ratings : 0,
                Reviews = reviewIds.Count,
            });
        }
    }
}
