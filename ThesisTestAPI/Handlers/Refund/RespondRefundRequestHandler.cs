using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Enum;
using ThesisTestAPI.Models.Midtrans;
using ThesisTestAPI.Models.Refunds;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Refund
{
    public class RespondRefundRequestHandler : IRequestHandler<RespondRefundRequest, (ProblemDetails?, RefundResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly MidtransService _midtransService;
        public RespondRefundRequestHandler(ThesisDbContext db, MidtransService midtransService, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _midtransService = midtransService;
            _httpContextAccessor = httpContextAccessor;
        }
        private ProblemDetails ProblemDetailTemplate(string detail)
        {
            return new ProblemDetails
            {
                Type = "http://veryCoolAPI.com/errors/invalid-data",
                Title = "Respond refund request error",
                Detail = detail,
                Instance = _httpContextAccessor.HttpContext?.Request.Path
            };
        }

        public async Task<(ProblemDetails?, RefundResponse?)> Handle(RespondRefundRequest request, CancellationToken cancellationToken)
        {
            if(request.Answer!= RequestStatuses.DECLINED && request.Answer != RequestStatuses.ACCEPTED)
            {
                return (ProblemDetailTemplate("Answer must be 'Declined' or 'Accepted'"), null);
            }
            var refundRequest = await _db.RefundRequests.Include(q=>q.Process).Include(q=>q.RefundRequestNavigation).Where(q => q.RefundRequestId == request.RefundRequestId && q.Status == RequestStatuses.PENDING).FirstOrDefaultAsync();
            if(refundRequest == null)
            {
                return (ProblemDetailTemplate("Refund request doesn't exist"), null);
            }
            if(request.Answer == RequestStatuses.DECLINED)
            {
                refundRequest.Status = RequestStatuses.DECLINED;
                await _db.SaveChangesAsync();
                return (null, new RefundResponse { RefundId = refundRequest.RefundRequestId});
            }
            //get amount by querying the total price of the process
            var midtransStatuses= new List<MidtransStatus>();
            long amountReturnedByWallet = 0;
            var steps = await _db.Steps.Include(q => q.Transaction).Where(q => q.ProcessId == refundRequest.ProcessId && (q.Status == StepStatuses.COMPLETED ||q.Status == StepStatuses.WORKING)).ToListAsync();
            foreach(var step in steps)
            {
                if (step.Transaction != null)
                {
                    if(step.Transaction.ExternalRef != null)
                    {
                        var status = await _midtransService.GetStatusAsync(step.Transaction.ExternalRef);
                        if (status != null && status.status_code != "404")
                        {
                            midtransStatuses.Add(status);
                        }
                    }
                    else
                    {
                        amountReturnedByWallet += step.Transaction.AmountMinor;
                    }
                }
            }
            var buyerWallet = await _db.Wallets.Where(q => q.UserId == refundRequest.RefundRequestNavigation.AuthorId).FirstOrDefaultAsync();
            if (buyerWallet == null)
            {
                return (ProblemDetailTemplate("Buyer wallet doesn't exist"), null);
            }

            if (amountReturnedByWallet > 0)
            {
                var walletTransaction = new WalletTransaction
                {
                    TransactionId = Guid.NewGuid(),
                    WalletId = buyerWallet.WalletId,
                    AmountMinor = amountReturnedByWallet,
                    CreatedAt = DateTimeOffset.Now,
                    IdempotencyKey = Guid.NewGuid().ToString(),
                    Type = "Refund",
                    Status = TransactionStatuses.POSTED,
                    PostedAt = DateTime.Now,
                    Direction = "C",
                    SignedAmount = amountReturnedByWallet,
                    ReferenceType = "Wallet",
                    Memo = "Process refund via wallet"
                };
                buyerWallet.BalanceMinor += amountReturnedByWallet;
                _db.WalletTransactions.Add(walletTransaction);
            }
            foreach (var status in midtransStatuses)
            {
                var grossAmount = (long)Convert.ToDouble(status.gross_amount);
                await _midtransService.CreateMidtransRefundAsync(status.transaction_id, grossAmount, refundRequest.Message);
                var snapTransaction = new WalletTransaction
                {
                    TransactionId = Guid.NewGuid(),
                    WalletId = buyerWallet.WalletId,
                    AmountMinor = grossAmount,
                    Direction = "C",
                    SignedAmount = grossAmount,
                    Type = "Refund",
                    Status = TransactionStatuses.PENDING,
                    CreatedAt = DateTimeOffset.Now,
                    IdempotencyKey = Guid.NewGuid().ToString(),
                    ExternalRef = status.order_id,
                    ReferenceType = "MidtransRefund",
                    Memo = "Refund via Midtrans",
                    //ReferenceId = refundRequest.RefundRequestId
                };
                _db.WalletTransactions.Add(snapTransaction);
            }
            var finalStep = steps.Where(q => q.ProcessId == refundRequest.ProcessId).OrderByDescending(q => q.CreatedAt).FirstOrDefault();
            refundRequest.Status = RequestStatuses.ACCEPTED;
            refundRequest.UpdatedAt = DateTimeOffset.Now;
            finalStep.Status = StepStatuses.CANCELLED;
            finalStep.UpdatedAt = DateTimeOffset.Now;
            refundRequest.Process.Status = ProcessStatuses.CANCELLED;
            refundRequest.Process.UpdatedAt = DateTimeOffset.Now;
            await _db.SaveChangesAsync();
            return (null, new RefundResponse { RefundId = refundRequest.RefundRequestId });
        }
    }
}
