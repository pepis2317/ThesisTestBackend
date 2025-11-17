using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Refunds
{
    public class GetRefundRequest : IRequest<(ProblemDetails?, PaginatedRefundRequestResponse?)>
    {
        public int pageSize { get; set; }
        public int pageNumber {  get; set; }
    }
}
