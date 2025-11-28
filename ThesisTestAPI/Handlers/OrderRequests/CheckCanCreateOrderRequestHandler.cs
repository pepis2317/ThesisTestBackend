using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Enum;
using ThesisTestAPI.Models.OrderRequests;

namespace ThesisTestAPI.Handlers.OrderRequests
{
    public class CheckCanCreateOrderRequestHandler : IRequestHandler<CheckCanCreateOrderRequest, (ProblemDetails?, bool?)>
    {
        private readonly ThesisDbContext _db;
        public CheckCanCreateOrderRequestHandler(ThesisDbContext db)
        {
            _db = db;
        }

        public async Task<(ProblemDetails?, bool?)> Handle(CheckCanCreateOrderRequest request, CancellationToken cancellationToken)
        {
            var isOwner = await _db.Requests.Include(q => q.Seller).Where(q => q.Seller.OwnerId == request.UserId && q.SellerId == request.SellerId).AnyAsync();
            if (!isOwner)
            {
                var activeProcesses = await _db.Processes.Include(q => q.Request).ThenInclude(q => q.RequestNavigation)
                    .Where(q => q.Request.RequestNavigation.AuthorId == request.UserId && q.Request.SellerId == request.SellerId && q.Status!=ProcessStatuses.COMPLETED && q.Status!=ProcessStatuses.CANCELLED).AnyAsync();
                return (null,  !activeProcesses);
            }
            return (null, false);
        }
    }
}
