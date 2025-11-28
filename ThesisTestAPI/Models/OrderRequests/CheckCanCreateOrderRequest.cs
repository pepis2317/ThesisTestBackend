using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.OrderRequests
{
    public class CheckCanCreateOrderRequest:IRequest<(ProblemDetails?, bool?)>
    {
        public Guid UserId { get; set; }
        public Guid SellerId { get; set; }
    }
}
