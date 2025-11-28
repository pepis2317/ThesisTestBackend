using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.OrderRequests
{
    public class GetOrderRequest:IRequest<(ProblemDetails?, OrderRequestResponse?)>
    {
        public Guid RequestId {  get; set; }
    }
}
