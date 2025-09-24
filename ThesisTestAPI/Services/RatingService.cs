using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Rating;

namespace ThesisTestAPI.Services
{
    public class RatingService
    {
        private readonly ThesisDbContext _db;
        public RatingService(ThesisDbContext db)
        {
            _db = db;
        }
        public async Task<RatingResponse> CreateRating (CreateRatingRequest request)
        {
            var reactionId = Guid.NewGuid();
            var Reaction = new Reaction
            {
                ReactionId = reactionId,
                ContentId = request.ContentId,
                AuthorId = request.AuthorId,
                CreatedAt = DateTimeOffset.Now
            };
            var Rating = new ThesisTestAPI.Entities.Rating
            {
                RatingId = reactionId,
                Rating1 = request.Rating,
                RatingNavigation = Reaction
            };
            _db.Ratings.Add(Rating);
            await _db.SaveChangesAsync();
            return new RatingResponse
            {
                AuthorId = request.AuthorId,
                RatingId = reactionId,
                Rating = (double)Rating.Rating1
            };
        }
        public async Task<string> DeleteRating(DeleteRatingRequest request)
        {
            await _db.Reactions.Where(q => q.ReactionId == request.RatingId).ExecuteDeleteAsync();
            return "Successfully deleted rating";
        }
        public async Task<RatingResponse?> EditRating(EditRatingRequest request)
        {
            var rating = await _db.Ratings.Include(q=>q.RatingNavigation).Where(q => q.RatingId == request.RatingId).FirstOrDefaultAsync();
            if(rating == null)
            {
                return null;
            }
            rating.Rating1 = request.Rating;
            rating.RatingNavigation.UpdatedAt = DateTimeOffset.Now;
            _db.Ratings.Update(rating);
            await _db.SaveChangesAsync();
            return new RatingResponse
            {
                AuthorId = rating.RatingNavigation.AuthorId,
                RatingId = rating.RatingId,
                Rating = (double)rating.Rating1
            };
        }
        public async Task<double> GetRating(GetRatingRequest request)
        {
            var average = await _db.Ratings
                .Where(r => r.RatingNavigation.ContentId == request.ContentId)
                .AverageAsync(r => r.Rating1);
            return (double)average;
        }
    }
}
