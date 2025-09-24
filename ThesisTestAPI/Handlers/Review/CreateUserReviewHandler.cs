using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Review;

namespace ThesisTestAPI.Handlers.Review
{
    public class CreateUserReviewHandler : IRequestHandler<CreateUserReviewRequest, (ProblemDetails?, string?)>
    {
        private readonly ThesisDbContext _db;
        public CreateUserReviewHandler(ThesisDbContext db)
        {
            _db = db;
        }
        public async Task<(ProblemDetails?, string?)> Handle(CreateUserReviewRequest request, CancellationToken cancellationToken)
        {
            var contentId = Guid.NewGuid();
            var content = new Content
            {
                ContentId = contentId,
                AuthorId = request.AuthorId,
                CreatedAt = DateTimeOffset.UtcNow,
            };
            var review = new UserReview
            {
                UserReviewId = contentId,
                UserReviewNavigation = content,
                Review = request.Review,
                UserId = request.UserId
            };
            _db.UserReviews.Add(review);
            await _db.SaveChangesAsync();
            return (null, contentId.ToString());
        }
    }
}
