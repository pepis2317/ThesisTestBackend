using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Enum;
using ThesisTestAPI.Models.OrderRequests;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.OrderRequests
{
    public class CreateOrderRequestHandler : IRequestHandler<CreateOrderRequest, (ProblemDetails?, OrderRequestResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly NotificationService _notificationService;
        public CreateOrderRequestHandler(ThesisDbContext db, NotificationService notificationService)
        {
            _db = db;
            _notificationService = notificationService;
        }
        public async Task<(ProblemDetails?, OrderRequestResponse?)> Handle(CreateOrderRequest request, CancellationToken cancellationToken)
        {
            var contentId = Guid.NewGuid();
            var content = new Content
            {
                ContentId = contentId,
                AuthorId = request.AuthorId,
                CreatedAt = DateTimeOffset.Now
            };
            var orderRequest = new ThesisTestAPI.Entities.Request
            {
                RequestId = contentId,
                SellerId = request.SellerId,
                RequestMessage = request.Message,
                RequestTitle = request.Title,
                RequestStatus = RequestStatuses.PENDING,
                RequestNavigation = content
            };
            _db.Requests.Add(orderRequest);
            var seller = await _db.Sellers.Where(q => q.SellerId == request.SellerId).FirstOrDefaultAsync();
            var sender = await _db.Users.Where(q=>q.UserId == request.AuthorId).FirstOrDefaultAsync();
            await _db.SaveChangesAsync();
            await _notificationService.SendNotification($"{sender.UserName} Sent a new message", seller.OwnerId);
            return (null, new OrderRequestResponse
            {
                RequestId = contentId,
                Status = orderRequest.RequestStatus,
            });
        }
    }
}
