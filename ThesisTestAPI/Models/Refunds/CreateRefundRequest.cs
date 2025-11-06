using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Refunds
{
    public class CreateRefundRequest: IRequest<(ProblemDetails?, RefundResponse?)>
    {
        public Guid AuthorId { get; set; }
        public Guid ProcessId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
