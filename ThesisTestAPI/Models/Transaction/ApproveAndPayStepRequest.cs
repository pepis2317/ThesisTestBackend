using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Transaction
{
    public class ApproveAndPayStepRequest : IRequest<(ProblemDetails?, TransactionResponse?)>
    {
        public Guid StepId { get; set; }
        public string Method { get; set; } = string.Empty;
    }
}
