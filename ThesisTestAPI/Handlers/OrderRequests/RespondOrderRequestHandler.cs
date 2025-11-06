using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
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
        private readonly NotificationService _notificationService;
        public RespondOrderRequestHandler(ThesisDbContext db, NotificationService notificationService, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _notificationService = notificationService;
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
            var orderRequest = await _db.Requests.Include(q=>q.RequestNavigation).Include(q=>q.Seller).Where(q => q.RequestId == request.RequestId).FirstOrDefaultAsync();
            if (orderRequest == null)
            {
                return (ProblemDetailTemplate("Order doesn't exist"), null);
            }
            orderRequest.RequestStatus = request.Status;
            _db.Requests.Update(orderRequest);            
            await _db.SaveChangesAsync();
            var receiver = orderRequest.RequestNavigation.AuthorId;
            await _notificationService.SendNotification($"{orderRequest.Seller.SellerName} Has {request.Status} your request", receiver);
            return (null, new OrderRequestResponse
            {
                RequestId = orderRequest.RequestId,
                Status = orderRequest.RequestStatus
            });
        }
    }
}
