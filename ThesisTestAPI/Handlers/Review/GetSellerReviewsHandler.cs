using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Review;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Review
{
    public class GetSellerReviewsHandler : IRequestHandler<GetSellerReviewsRequest, (ProblemDetails?, PaginatedReviewsResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly BlobStorageService _blobStorageService;
        public GetSellerReviewsHandler(ThesisDbContext db, BlobStorageService blobStorageService)
        {
            _db = db;
            _blobStorageService = blobStorageService;
        }

        public async Task<(ProblemDetails?, PaginatedReviewsResponse?)> Handle(GetSellerReviewsRequest request, CancellationToken cancellationToken)
        {
            var reviews = await _db.SellerReviews
                .Include(q=>q.SellerReviewNavigation).ThenInclude(q=>q.Author)
                .Skip((request.pageNumber - 1) * request.pageSize)
                .Where(q => q.SellerId == request.SellerId).OrderByDescending(q => q.SellerReviewNavigation.CreatedAt).ToListAsync();
            var reviewContentIds = reviews.Select(q=>q.SellerReviewNavigation.ContentId).ToList();
            var ratings = await _db.Ratings.Include(q=>q.RatingNavigation).Where(q=> reviewContentIds.Contains(q.RatingNavigation.ContentId)).ToListAsync();
            var commentCounts = await _db.Comments
                .Where(q => reviewContentIds.Contains(q.TargetContentId))
                .GroupBy(q => q.TargetContentId)
                .Select(g => new
                {
                    TargetContentId = g.Key,
                    CommentCount = g.Count()
                })
                .ToListAsync();
            var likes = await _db.Likes.Include(q => q.LikeNavigation).Where(q => reviewContentIds.Contains(q.LikeNavigation.ContentId))
                .GroupBy(q => q.LikeNavigation.ContentId).Select(q => new
                {
                    ContentId = q.Key,
                    Likes = q.Count()
                }).ToListAsync();
            var liked = await _db.Likes.Include(q => q.LikeNavigation).Where(q => reviewContentIds.Contains(q.LikeNavigation.ContentId) && q.LikeNavigation.AuthorId == request.UserId).ToListAsync();
            var list = new List<ReviewResponse>();
            foreach(var review in reviews)
            {
                var isLiked = liked.Where(q => q.LikeNavigation.ContentId == review.SellerReviewId).Any();
                var likeCount = likes.Where(q => q.ContentId == review.SellerReviewId).Select(q => q.Likes).FirstOrDefault();
                var commentCount = commentCounts.Where(q => q.TargetContentId == review.SellerReviewNavigation.ContentId).FirstOrDefault();
                var rating = ratings.Where(q => q.RatingNavigation.ContentId == review.SellerReviewNavigation.ContentId).FirstOrDefault();
                var user = review.SellerReviewNavigation.Author;
                var pfp = "";
                if (!string.IsNullOrEmpty(user.Pfp))
                {
                    pfp = await _blobStorageService.GetTemporaryImageUrl(user.Pfp, Enum.BlobContainers.PFP);
                }
                list.Add(new ReviewResponse
                {
                    ReviewId = review.SellerReviewId,
                    AuthorId = user.UserId,
                    AuthorName = user.UserName,
                    AuthorPfp = pfp,
                    Review = review.Review,
                    CreatedAt = review.SellerReviewNavigation.CreatedAt,
                    UpdatedAt = review.SellerReviewNavigation.UpdatedAt,
                    Rating = rating != null? rating.Rating1:0,
                    Comments = commentCount != null ? commentCount.CommentCount : 0,
                    Likes = likeCount,
                    Liked = isLiked
                });
            }
            var total = await _db.SellerReviews.Where(q => q.SellerId == request.SellerId).CountAsync();
            return (null, new PaginatedReviewsResponse
            {
                Total = total,
                Reviews = list
            });
        }
    }
}
