using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Refunds
{
    public class GetRefundRequestById: IRequest<(ProblemDetails?, RefundResponse?)>
    {
        public Guid RefundRequestId { get; set; }
    }
}
