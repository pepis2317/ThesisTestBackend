using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Review;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Review
{
    public class GetUserReviewHandler : IRequestHandler<GetUserReviewRequest, (ProblemDetails?, List<ReviewResponse>?)>
    {
        private readonly ThesisDbContext _db;
        private readonly BlobStorageService _blobStorageService;
        public GetUserReviewHandler(ThesisDbContext db, BlobStorageService blobStorageService)
        {
            _db = db;
            _blobStorageService = blobStorageService;
        }

        public async Task<(ProblemDetails?, List<ReviewResponse>?)> Handle(GetUserReviewRequest request, CancellationToken cancellationToken)
        {
            var reviews = await _db.UserReviews.OrderByDescending(q => q.UserReviewNavigation.CreatedAt).Where(q => q.UserId == request.UserId).ToListAsync();
            var myReviews = reviews.Where(q => q.UserReviewNavigation.AuthorId == request.UserId).ToList();
            var otherReviews = reviews.Where(q => q.UserReviewNavigation.AuthorId != request.UserId).ToList();
            var reviewsList = new List<ReviewResponse>();
            if (myReviews.Count() > 0)
            {
                var producer = await _db.Producers.Where(q => q.ProducerId == request.AuthorId).FirstOrDefaultAsync();
                var authorPfp = await _blobStorageService.GetTemporaryImageUrl(producer.ProducerPicture, Enum.BlobContainers.PRODUCERPICTURE);
                foreach (var review in myReviews)
                {
                    var comments = await _db.Comments.Where(q => q.CommentNavigation.ContentId == review.UserReviewId).CountAsync();
                    var reviewResponse = new ReviewResponse
                    {
                        ReviewId = review.UserReviewId,
                        AuthorId = producer.ProducerId,
                        AuthorName = producer.ProducerName,
                        AuthorPfp = authorPfp,
                        Review = review.Review,
                        CreatedAt = review.UserReviewNavigation.CreatedAt,
                        UpdatedAt = review.UserReviewNavigation.UpdatedAt,
                        Comments = comments
                    };
                    reviewsList.Add(reviewResponse);
                }
            }
            foreach (var review in otherReviews)
            {
                var producer = await _db.Producers.Where(q => q.ProducerId == request.AuthorId).FirstOrDefaultAsync();
                var authorPfp = await _blobStorageService.GetTemporaryImageUrl(producer.ProducerPicture, Enum.BlobContainers.PRODUCERPICTURE);
                var comments = await _db.Comments.Where(q => q.CommentNavigation.ContentId == review.UserReviewId).CountAsync();
                var reviewResponse = new ReviewResponse
                {
                    AuthorId = producer.ProducerId,
                    AuthorName = producer.ProducerName,
                    AuthorPfp = authorPfp,
                    Review = review.Review,
                    CreatedAt = review.UserReviewNavigation.CreatedAt,
                    UpdatedAt = review.UserReviewNavigation.UpdatedAt,
                    Comments = comments
                };
                reviewsList.Add(reviewResponse);
            }
            return (null, reviewsList);
        }
    }
}
