using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Review;

namespace ThesisTestAPI.Handlers.Review
{
    public class CreateProducerReviewHandler : IRequestHandler<CreateProducerReviewRequest, (ProblemDetails?, string?)>
    {
        private readonly ThesisDbContext _db;
        public CreateProducerReviewHandler(ThesisDbContext db)
        {
            _db = db;
        }
        public async Task<(ProblemDetails?, string?)> Handle(CreateProducerReviewRequest request, CancellationToken cancellationToken)
        {
            var contentId = Guid.NewGuid();
            var content = new Content
            {
                ContentId = contentId,
                AuthorId = request.AuthorId,
                CreatedAt = DateTimeOffset.UtcNow,
            };
            var review = new ProducerReview
            {
                ProducerReviewId = contentId,
                ProducerReviewNavigation = content,
                Review = request.Review,
                ProducerId = request.ProducerId
            };
            _db.ProducerReviews.Add(review);
            await _db.SaveChangesAsync();
            return (null, contentId.ToString());
        }
    }
}
