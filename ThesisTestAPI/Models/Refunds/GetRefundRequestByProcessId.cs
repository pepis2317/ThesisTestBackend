using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Refunds
{
    public class GetRefundRequestByProcessId: IRequest<(ProblemDetails?, RefundResponse?)>
    {
        public Guid ProcessId { get; set; }
    }
}
