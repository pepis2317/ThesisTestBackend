using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Producer;
using ThesisTestAPI.Models.Review;

namespace ThesisTestAPI.Handlers.Review
{
    public class EditProducerReviewHandler : IRequestHandler<EditProducerReviewRequest, (ProblemDetails?, string?)>
    {
        private readonly ThesisDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public EditProducerReviewHandler(ThesisDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<(ProblemDetails?, string?)> Handle(EditProducerReviewRequest request, CancellationToken cancellationToken)
        {
            var review = await _db.ProducerReviews.Where(q => q.ProducerReviewId == request.ReviewId).FirstOrDefaultAsync();
            if(review == null)
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
            review.Review = request.Review;
            review.ProducerReviewNavigation.UpdatedAt = DateTimeOffset.Now;
            _db.ProducerReviews.Update(review);
            await _db.SaveChangesAsync();
            return (null, $@"Successfully Updated {review.ProducerReviewId}");
        }
    }
}
