using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Review;

namespace ThesisTestAPI.Handlers.Review
{
    public class CheckReviewUserHandler : IRequestHandler<CheckReviewUser, (ProblemDetails?, bool?)>
    {
        private readonly ThesisDbContext _db;
        public CheckReviewUserHandler(ThesisDbContext db)
        {
            _db = db;
        }

        public async Task<(ProblemDetails?, bool?)> Handle(CheckReviewUser request, CancellationToken cancellationToken)
        {
            var process = await _db.Processes
               .Include(q => q.Request).ThenInclude(q => q.RequestNavigation)
               .Include(q => q.Request).ThenInclude(q => q.Seller)
               .Where(q => q.Request.Seller.OwnerId == request.AuthorId && q.Request.RequestNavigation.AuthorId == request.UserId
               && (q.Status == Enum.ProcessStatuses.COMPLETED || q.Status == Enum.ProcessStatuses.CANCELLED)).AnyAsync();
            return (null, process);
        }
    }
}
