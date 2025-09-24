using Azure.Core;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Like;

namespace ThesisTestAPI.Services
{
    public class LikesService
    {
        private readonly ThesisDbContext _db;
        public LikesService( ThesisDbContext db)
        {
            _db = db;
        }
        public async Task<LikesResponse> GetLikes(GetLikesRequest request)
        {
            var likes = await _db.Likes.Where(q => q.LikeNavigation.ContentId == request.ContentId).CountAsync();
            var myLike = await _db.Likes.Where(q => q.LikeNavigation.AuthorId == request.AuthorId && q.LikeNavigation.ContentId == request.ContentId).FirstOrDefaultAsync();
            return new LikesResponse
            {
                Likes = likes,
                MyLikeId = myLike?.LikeId
            };
        }
        public async Task<LikeResponse> Like(LikeRequest request)
        {
            var ReactionId = Guid.NewGuid();
            var reaction = new Reaction
            {
                ReactionId = ReactionId,
                ContentId = request.ContentId,
                AuthorId = request.AuthorId,
                CreatedAt = DateTimeOffset.Now
            };
            var like = new ThesisTestAPI.Entities.Like
            {
                LikeId = ReactionId,
                LikeNavigation = reaction
            };
            _db.Likes.Add(like);
            await _db.SaveChangesAsync();
            return new LikeResponse
            {
                LikeId = like.LikeId
            };
        }
        public async Task<string> Unlike(UnlikeRequest request)
        {
            await _db.Reactions.Where(q => q.ReactionId == request.LikeId).ExecuteDeleteAsync();
            return "Successfully unliked";
        }
    }
}
