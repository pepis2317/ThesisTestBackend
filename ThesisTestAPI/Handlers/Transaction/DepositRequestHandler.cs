using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Enum;
using ThesisTestAPI.Models.Transaction;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Transaction
{
    public class DepositRequestHandler : IRequestHandler<DepositRequest, (ProblemDetails?, TransactionResponse?)>
    {
        private readonly MidtransService _midtransService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ThesisDbContext _db;
        public DepositRequestHandler(MidtransService midtransService, IHttpContextAccessor httpContextAccessor, ThesisDbContext db)
        {
            _midtransService = midtransService;
            _httpContextAccessor = httpContextAccessor;
            _db = db;
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
        public async Task<(ProblemDetails?, TransactionResponse?)> Handle(DepositRequest request, CancellationToken cancellationToken)
        {
            if (request.Amount <= 0)
            {
                return (ProblemDetailTemplate("Amount cant be <= 0"), null);
            }
            var wallet = await _db.Wallets.Include(q=>q.User).Where(q => q.UserId == request.UserId).FirstOrDefaultAsync();
            if(wallet == null)
            {
                return (ProblemDetailTemplate("Wallet doesn't exist"), null);
            }
            var orderId = $"deposit-{Guid.NewGuid()}";
            var transaction = new WalletTransaction
            {
                TransactionId = Guid.NewGuid(),
                WalletId = wallet.WalletId,
                AmountMinor = request.Amount,
                Direction = "C",
                SignedAmount = request.Amount,
                Type = "Deposit",
                Status = TransactionStatuses.PENDING,
                CreatedAt = DateTimeOffset.Now,
                IdempotencyKey = Guid.NewGuid().ToString(),
                ExternalRef = orderId,
                ReferenceType = "MidtransSnap",
                Memo = "Deposit via Midtrans Snap"
            };
            _db.WalletTransactions.Add(transaction);
            await _db.SaveChangesAsync();
            var snap = await _midtransService.CreateSnapTransactionAsync(orderId, request.Amount, wallet.User.Email, wallet.User.UserName);
            if(snap == null)
            {
                return (ProblemDetailTemplate("Something wrong with creating the midtrans transaction"), null);
            }
            return (null, new TransactionResponse
            {
                orderId = orderId,
                token = snap.token,
                redirectUrl = snap.redirect_url
            });
        }
    }
}
