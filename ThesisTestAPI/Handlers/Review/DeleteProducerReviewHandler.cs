using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Review;

namespace ThesisTestAPI.Handlers.Review
{
    public class DeleteProducerReviewHandler : IRequestHandler<DeleteProducerReviewRequest, (ProblemDetails?, string?)>
    {
        private readonly ThesisDbContext _db;
        public DeleteProducerReviewHandler(ThesisDbContext db)
        {
            _db = db;
        }
        public async Task<(ProblemDetails?, string?)> Handle(DeleteProducerReviewRequest request, CancellationToken cancellationToken)
        {
            await _db.Contents.Where(q => q.ContentId == request.ReviewId).ExecuteDeleteAsync();
            return (null, "Successfully deleted review");
        }
    }
}
