using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Review;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Review
{
    public class GetProducerReviewHandler : IRequestHandler<GetProducerReviewRequest, (ProblemDetails?, List<ReviewResponse>?)>
    {
        private readonly ThesisDbContext _db;
        private readonly BlobStorageService _blobStorageService;
        public GetProducerReviewHandler(ThesisDbContext db, BlobStorageService blobStorageService)
        {
            _db = db;
            _blobStorageService = blobStorageService;
        }

        public async Task<(ProblemDetails?, List<ReviewResponse>?)> Handle(GetProducerReviewRequest request, CancellationToken cancellationToken)
        {
            var reviews = await _db.ProducerReviews.OrderByDescending(q=>q.ProducerReviewNavigation.CreatedAt).Where(q=>q.ProducerId == request.ProducerId).ToListAsync();
            var myReviews = reviews.Where(q => q.ProducerReviewNavigation.AuthorId == request.ProducerId).ToList();
            var otherReviews = reviews.Where(q => q.ProducerReviewNavigation.AuthorId != request.ProducerId).ToList();
            var reviewsList = new List<ReviewResponse>();
            if (myReviews.Count() > 0)
            {
                var user = await _db.Users.Where(q => q.UserId == request.AuthorId).FirstOrDefaultAsync();
                var authorPfp = await _blobStorageService.GetTemporaryImageUrl(user.Pfp, Enum.BlobContainers.PFP);
                foreach (var review in myReviews)
                {
                    var comments = await _db.Comments.Where(q => q.CommentNavigation.ContentId == review.ProducerReviewId).CountAsync();
                    var reviewResponse = new ReviewResponse
                    {
                        ReviewId = review.ProducerReviewId,
                        AuthorId = user.UserId,
                        AuthorName = user.UserName,
                        AuthorPfp = authorPfp,
                        Review = review.Review,
                        CreatedAt = review.ProducerReviewNavigation.CreatedAt,
                        UpdatedAt = review.ProducerReviewNavigation.UpdatedAt,
                        Comments = comments
                    };
                    reviewsList.Add(reviewResponse);
                }
            }
            foreach(var review in otherReviews)
            {
                var user = await _db.Users.Where(q => q.UserId == review.ProducerReviewNavigation.AuthorId).FirstOrDefaultAsync();
                var authorPfp = await _blobStorageService.GetTemporaryImageUrl(user.Pfp, Enum.BlobContainers.PFP);
                var comments = await _db.Comments.Where(q => q.CommentNavigation.ContentId == review.ProducerReviewId).CountAsync();
                var reviewResponse = new ReviewResponse
                {
                    AuthorId = user.UserId,
                    AuthorName = user.UserName,
                    AuthorPfp = authorPfp,
                    Review = review.Review,
                    CreatedAt = review.ProducerReviewNavigation.CreatedAt,
                    UpdatedAt = review.ProducerReviewNavigation.UpdatedAt,
                    Comments = comments
                };
                reviewsList.Add(reviewResponse);
            }
            return (null, reviewsList);
        }
    }
}
