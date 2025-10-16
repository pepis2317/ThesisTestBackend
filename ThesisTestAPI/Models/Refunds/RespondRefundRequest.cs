using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Refunds
{
    public class RespondRefundRequest: IRequest<(ProblemDetails?, RefundResponse?)>
    {
        public Guid RefundRequestId { get; set; }
        public string Answer { get; set; } = string.Empty;
    }
}
