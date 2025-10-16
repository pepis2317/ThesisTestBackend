using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Enum;
using ThesisTestAPI.Models.OrderRequests;

namespace ThesisTestAPI.Handlers.OrderRequests
{
    public class CreateOrderRequestHandler : IRequestHandler<CreateOrderRequest, (ProblemDetails?, OrderRequestResponse?)>
    {
        private readonly ThesisDbContext _db;
        public CreateOrderRequestHandler(ThesisDbContext db)
        {
            _db = db;
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
            await _db.SaveChangesAsync();
            return (null, new OrderRequestResponse
            {
                RequestId = contentId,
                Status = orderRequest.RequestStatus,
            });
        }
    }
}
