using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.OrderRequests
{
    public class CreateOrderRequest : IRequest<(ProblemDetails?, OrderRequestResponse?)>
    {
        public Guid AuthorId { get; set; }
        public Guid SellerId { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
    }
}
