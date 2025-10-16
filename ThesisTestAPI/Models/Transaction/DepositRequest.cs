using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Transaction
{
    public class DepositRequest: IRequest<(ProblemDetails?, TransactionResponse?)>
    {
        public long Amount { get; set; }
        public Guid UserId { get; set; }
    }
}
