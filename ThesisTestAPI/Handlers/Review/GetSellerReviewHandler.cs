using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Review;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Review
{
    public class GetSellerReviewHandler : IRequestHandler<GetSellerReviewRequest, (ProblemDetails?, List<ReviewResponse>?)>
    {
        private readonly ThesisDbContext _db;
        private readonly BlobStorageService _blobStorageService;
        public GetSellerReviewHandler(ThesisDbContext db, BlobStorageService blobStorageService)
        {
            _db = db;
            _blobStorageService = blobStorageService;
        }

        public async Task<(ProblemDetails?, List<ReviewResponse>?)> Handle(GetSellerReviewRequest request, CancellationToken cancellationToken)
        {
            var reviews = await _db.SellerReviews.OrderByDescending(q=>q.SellerReviewNavigation.CreatedAt).Where(q=>q.SellerId == request.SellerId).ToListAsync();
            var myReviews = reviews.Where(q => q.SellerReviewNavigation.AuthorId == request.SellerId).ToList();
            var otherReviews = reviews.Where(q => q.SellerReviewNavigation.AuthorId != request.SellerId).ToList();
            var reviewsList = new List<ReviewResponse>();
            if (myReviews.Count() > 0)
            {
                var user = await _db.Users.Where(q => q.UserId == request.AuthorId).FirstOrDefaultAsync();
                var authorPfp = await _blobStorageService.GetTemporaryImageUrl(user.Pfp, Enum.BlobContainers.PFP);
                foreach (var review in myReviews)
                {
                    var comments = await _db.Comments.Where(q => q.CommentNavigation.ContentId == review.SellerReviewId).CountAsync();
                    var reviewResponse = new ReviewResponse
                    {
                        ReviewId = review.SellerReviewId,
                        AuthorId = user.UserId,
                        AuthorName = user.UserName,
                        AuthorPfp = authorPfp,
                        Review = review.Review,
                        CreatedAt = review.SellerReviewNavigation.CreatedAt,
                        UpdatedAt = review.SellerReviewNavigation.UpdatedAt,
                        Comments = comments
                    };
                    reviewsList.Add(reviewResponse);
                }
            }
            foreach(var review in otherReviews)
            {
                var user = await _db.Users.Where(q => q.UserId == review.SellerReviewNavigation.AuthorId).FirstOrDefaultAsync();
                var authorPfp = await _blobStorageService.GetTemporaryImageUrl(user.Pfp, Enum.BlobContainers.PFP);
                var comments = await _db.Comments.Where(q => q.CommentNavigation.ContentId == review.SellerReviewId).CountAsync();
                var reviewResponse = new ReviewResponse
                {
                    AuthorId = user.UserId,
                    AuthorName = user.UserName,
                    AuthorPfp = authorPfp,
                    Review = review.Review,
                    CreatedAt = review.SellerReviewNavigation.CreatedAt,
                    UpdatedAt = review.SellerReviewNavigation.UpdatedAt,
                    Comments = comments
                };
                reviewsList.Add(reviewResponse);
            }
            return (null, reviewsList);
        }
    }
}
