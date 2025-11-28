using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Review;

namespace ThesisTestAPI.Handlers.Review
{
    public class GetSellerStatsHandler : IRequestHandler<GetSellerStatsRequest, (ProblemDetails?, SellerStatsResponse?)>
    {
        private readonly ThesisDbContext _db;
        public GetSellerStatsHandler(ThesisDbContext db)
        {
            _db = db;
        }

        public async Task<(ProblemDetails?, SellerStatsResponse?)> Handle(GetSellerStatsRequest request, CancellationToken cancellationToken)
        {
            var reviewIds = await _db.SellerReviews.Where(q => q.SellerId == request.SellerId).Select(q => q.SellerReviewId).ToListAsync();
            var ratings = await _db.Ratings.Include(q => q.RatingNavigation).Where(q=>reviewIds.Contains(q.RatingNavigation.ContentId)).Select(q=>q.Rating1).AverageAsync();
            var uniqueClients = await _db.Requests.Where(r => r.SellerId == request.SellerId && r.RequestStatus == Enum.RequestStatuses.ACCEPTED).Select(r => r.RequestId).Distinct().CountAsync();
            var processes = await _db.Processes.Include(q => q.Request).Where(q => q.Request.SellerId == request.SellerId).ToListAsync();
            var completed = processes.Where(q => q.Status == Enum.ProcessStatuses.COMPLETED);
            double completionRate = 0;
            if (processes.Count() > 0)
            {
                completionRate = (double)completed.Count() / processes.Count();
            }
            return (null, new SellerStatsResponse
            {
                Rating = ratings != null? (double)ratings : 0,
                Clients = uniqueClients,
                Reviews = reviewIds.Count,
                CompletionRate = completionRate
            });
        }
    }
}
