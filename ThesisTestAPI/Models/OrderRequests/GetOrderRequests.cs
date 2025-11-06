using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.OrderRequests
{
    public class GetOrderRequests : IRequest<(ProblemDetails?, PaginatedOrderRequestsResponse?)>
    {
        public Guid UserId {  get; set; }
        public int pageSize { get; set; }
        public int pageNumber { get; set; }
    }
}
