using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Enum;
using ThesisTestAPI.Models.Transaction;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Transaction
{
    public class WithdrawHandler : IRequestHandler<WithdrawRequest, (ProblemDetails?, TransactionResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly MidtransService _midtransService;
        public WithdrawHandler(ThesisDbContext db, IHttpContextAccessor httpContextAccessor, MidtransService midtransService)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _midtransService = midtransService;
        }
        private ProblemDetails ProblemDetailTemplate(string detail)
        {
            return new ProblemDetails
            {
                Type = "http://veryCoolAPI.com/errors/invalid-data",
                Title = "Midtrans error",
                Detail = detail,
                Instance = _httpContextAccessor.HttpContext?.Request.Path
            };
        }
        public async Task<(ProblemDetails?, TransactionResponse?)> Handle(WithdrawRequest request, CancellationToken cancellationToken)
        {
            if (request.Amount <= 0)
            {
                return (ProblemDetailTemplate("Amount cant be <= 0"), null);
            }
            var wallet = await _db.Wallets.Include(q=>q.User).Where(q => q.UserId == request.UserId).FirstOrDefaultAsync();
            if (wallet == null)
            {
                return (ProblemDetailTemplate("Wallet doesn't exist"), null);
            }
            var referenceNo = $"payout-{Guid.NewGuid()}";
            var transaction = new WalletTransaction
            {
                TransactionId = Guid.NewGuid(),
                WalletId = wallet.WalletId,
                AmountMinor = request.Amount,
                Direction = "D",
                SignedAmount = -request.Amount,
                Type = "Withdrawal",
                Status = TransactionStatuses.PENDING,
                CreatedAt = DateTimeOffset.Now,
                IdempotencyKey = Guid.NewGuid().ToString(),
                ExternalRef = referenceNo,
                ReferenceType = "IrisPayout",
                Memo = "Payout via Midtrans Iris"
            };
            _db.WalletTransactions.Add(transaction);
            await _db.SaveChangesAsync();
            //var iris = await _midtransService.CreateWithdrawalAsync(referenceNo, request.Amount, request.BankCode, request.Account, request.Name, wallet.User.Email);
            //if (iris == null)
            //{
            //    return (ProblemDetailTemplate("Something wrong with creating the iris payout"), null);
            //}
            return (null, new TransactionResponse
            {
                orderId = referenceNo
            });
        }
    }
}
