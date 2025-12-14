using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Enum;
using ThesisTestAPI.Models.Transaction;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Transaction
{
    public class ApproveAndPayStepHandler : IRequestHandler<ApproveAndPayStepRequest, (ProblemDetails?, TransactionResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly MidtransService _midtransService;
        private readonly NotificationService _notificationService;
        public ApproveAndPayStepHandler(ThesisDbContext db, IHttpContextAccessor httpContextAccessor, MidtransService midtransService, NotificationService notificationService)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _midtransService = midtransService;
            _notificationService = notificationService;
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
        public async Task<(ProblemDetails?, TransactionResponse?)> Handle(ApproveAndPayStepRequest request, CancellationToken cancellationToken)
        {
            var step = await _db.Steps.Include(q=>q.Process)
                .Include(q=>q.Transaction)
                .Where(q => q.StepId == request.StepId).FirstOrDefaultAsync();
            if(step == null)
            {
                return (ProblemDetailTemplate("Step doesn't exist"), null);
            }
            var buyerId = await _db.Contents.Where(q => q.ContentId == step.Process.RequestId).Select(q => q.AuthorId).FirstOrDefaultAsync();
            var buyerWallet = await _db.Wallets.Include(q=>q.User).Where(q => q.UserId == buyerId).FirstOrDefaultAsync();
            if (buyerWallet == null)
            {
                return (ProblemDetailTemplate("Buyer wallet doesn't exist"), null);
            }
            var sellerId = await _db.Requests.Include(q => q.Seller).Where(q => q.RequestId == step.Process.RequestId).Select(q => q.Seller.OwnerId).FirstOrDefaultAsync();
            var sellerWallet = await _db.Wallets.Where(q => q.UserId == sellerId).FirstOrDefaultAsync();
            if (sellerWallet == null)
            {
                return (ProblemDetailTemplate("Seller wallet doesn't exist"), null);
            }
            if (request.Method == "Wallet")
            {             
                if (buyerWallet.BalanceMinor < step.Amount)
                {
                    return (ProblemDetailTemplate("Insufficient funds"), null);
                }
                var walletTransaction = new WalletTransaction
                {
                    TransactionId = Guid.NewGuid(),
                    WalletId = buyerWallet.WalletId,
                    AmountMinor = step.Amount,
                    CreatedAt = DateTimeOffset.Now,
                    IdempotencyKey = Guid.NewGuid().ToString(),
                    Type = "Fee",
                    Status = TransactionStatuses.POSTED,
                    PostedAt = DateTime.Now,
                    Direction = "D",
                    SignedAmount = -step.Amount,
                    ReferenceType = "Wallet",
                    Memo = "Step payment via wallet"
                };
                buyerWallet.BalanceMinor -= step.Amount;
                step.Status = StepStatuses.WORKING;
                step.UpdatedAt = DateTimeOffset.Now;
                step.TransactionId = walletTransaction.TransactionId;
                _db.WalletTransactions.Add(walletTransaction);
                await _db.SaveChangesAsync();
                await _notificationService.SendNotification("Step has been accepted", sellerId);
                return (null, new TransactionResponse
                {
                    orderId = walletTransaction.TransactionId.ToString(),
                    paymentStatus = TransactionStatuses.POSTED
                });
            }
            if (step.Transaction != null)
            {
                if (step.Transaction.Status == TransactionStatuses.POSTED)
                {
                    return (null, new TransactionResponse
                    {
                        orderId = step.Transaction.ExternalRef,
                        paymentStatus = TransactionStatuses.POSTED
                    });
                }
            }
            var orderId = $"fee-{Guid.NewGuid()}";
            var transaction = new WalletTransaction
            {
                TransactionId = Guid.NewGuid(),
                WalletId = sellerWallet.WalletId,
                AmountMinor = step.Amount,
                Direction = "C",
                SignedAmount = step.Amount,
                Type = "Fee",
                Status = TransactionStatuses.PENDING,
                CreatedAt = DateTimeOffset.Now,
                IdempotencyKey = Guid.NewGuid().ToString(),
                ExternalRef = orderId,
                ReferenceType = "MidtransSnap",
                Memo = "Payment via Midtrans Snap"
            };
            step.UpdatedAt = DateTimeOffset.Now;
            step.TransactionId = transaction.TransactionId;
            _db.WalletTransactions.Add(transaction);
            await _db.SaveChangesAsync();
            await _notificationService.SendNotification("Step has been accepted", sellerId);
            var snap = await _midtransService.CreateSnapTransactionAsync(orderId, step.Amount, email:buyerWallet.User.Email, firstName:buyerWallet.User.UserName);
            if (snap == null)
            {
                return (ProblemDetailTemplate("Something went wrong when creating midtrans transaction"), null);
            }
            return (null, new TransactionResponse
            {
                orderId = orderId,
                token = snap.token,
                redirectUrl = snap.redirect_url,
                paymentStatus = TransactionStatuses.PENDING
            });
        }
    }
}
