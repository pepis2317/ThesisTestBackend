using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Transaction
{
    public class WithdrawRequest : IRequest<(ProblemDetails?, TransactionResponse?)>
    {
        public long Amount { get; set; }
        public Guid UserId { get; set; }
        public string BankCode { get; set; } = string.Empty;
        public string Account { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

    }
}
