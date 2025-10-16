using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Transaction
{
    public class TestTransactionRequest : IRequest<(ProblemDetails?, TransactionResponse?)>
    {
        public long Amount { get; set; }
        public Guid WalletId { get; set; }
    }
}
