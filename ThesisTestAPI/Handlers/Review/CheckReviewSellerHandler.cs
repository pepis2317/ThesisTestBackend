using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Review;

namespace ThesisTestAPI.Handlers.Review
{
    public class CheckReviewSellerHandler : IRequestHandler<CheckReviewSeller, (ProblemDetails?, bool?)>
    {
        private readonly ThesisDbContext _db;
        public CheckReviewSellerHandler(ThesisDbContext db)
        {
            _db = db;
        }
        public async Task<(ProblemDetails?, bool?)> Handle(CheckReviewSeller request, CancellationToken cancellationToken)
        {
            var process = await _db.Processes
                .Include(q => q.Request).ThenInclude(q => q.RequestNavigation)
                .Where(q => q.Request.SellerId == request.SellerId && q.Request.RequestNavigation.AuthorId == request.AuthorId 
                && (q.Status == Enum.ProcessStatuses.COMPLETED || q.Status == Enum.ProcessStatuses.CANCELLED)).AnyAsync();
            return (null, process);
        }
    }
}
