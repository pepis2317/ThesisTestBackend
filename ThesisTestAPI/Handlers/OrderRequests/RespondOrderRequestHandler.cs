using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Enum;
using ThesisTestAPI.Models.OrderRequests;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.OrderRequests
{
    public class RespondOrderRequestHandler : IRequestHandler<RespondOrderRequest, (ProblemDetails?, OrderRequestResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RespondOrderRequestHandler(ThesisDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }
        private ProblemDetails ProblemDetailTemplate(string detail)
        {
            return new ProblemDetails
            {
                Type = "http://veryCoolAPI.com/errors/invalid-data",
                Title = "Invalid request data",
                Detail = detail,
                Instance = _httpContextAccessor.HttpContext?.Request.Path
            };
        }
        public async Task<(ProblemDetails?, OrderRequestResponse?)> Handle(RespondOrderRequest request, CancellationToken cancellationToken)
        {
            if(request.Status != RequestStatuses.ACCEPTED && request.Status != RequestStatuses.DECLINED)
            {
                return (ProblemDetailTemplate("Status must be Accepted or Declined"), null);
            }
            var orderRequest = await _db.Requests.Where(q => q.RequestId == request.RequestId).FirstOrDefaultAsync();
            if (orderRequest == null)
            {
                return (ProblemDetailTemplate("Order doesn't exist"), null);
            }
            orderRequest.RequestStatus = request.Status;
            _db.Requests.Update(orderRequest);
            await _db.SaveChangesAsync();

            return (null, new OrderRequestResponse
            {
                RequestId = orderRequest.RequestId,
                Status = orderRequest.RequestStatus
            });
        }
    }
}
