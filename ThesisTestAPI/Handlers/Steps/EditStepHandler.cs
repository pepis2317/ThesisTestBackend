using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Text;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Enum;
using ThesisTestAPI.Models.Midtrans;
using ThesisTestAPI.Models.Steps;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Steps
{
    public class EditStepHandler : IRequestHandler<EditStepRequest, (ProblemDetails?, StepResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly BlobStorageService _blobStorageService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly MidtransService _midtransService;
        public EditStepHandler(ThesisDbContext db, MidtransService midtransService, IHttpContextAccessor httpContextAccessor, BlobStorageService blobStorageService)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _midtransService = midtransService;
            _blobStorageService = blobStorageService;
        }
        private ProblemDetails ProblemDetailTemplate(string detail)
        {
            return new ProblemDetails
            {
                Type = "http://veryCoolAPI.com/errors/invalid-data",
                Title = "Edit step error",
                Detail = detail,
                Instance = _httpContextAccessor.HttpContext?.Request.Path
            };
        }
        public async Task<(ProblemDetails?, StepResponse?)> Handle(EditStepRequest request, CancellationToken cancellationToken)
        {
            var step = await _db.Steps.Include(q=>q.Transaction).Include(q=>q.Process).Where(q => q.StepId == request.StepId).FirstOrDefaultAsync();
            if(step == null)
            {
                return (ProblemDetailTemplate("Invalid step id"), null);
            }
            if (!string.IsNullOrEmpty(request.Title))
            {
                step.Title = request.Title;
            }
            if (!string.IsNullOrEmpty(request.Description))
            {
                step.Description = request.Description;
            }
            if (!string.IsNullOrEmpty(request.Status))
            {
                if(request.Status != StepStatuses.COMPLETED && request.Status != StepStatuses.CANCELLED)
                {
                    return (ProblemDetailTemplate("Invalid status update"), null);
                }
                if(request.Status == StepStatuses.CANCELLED && step.Status == StepStatuses.WORKING)
                {
                    var buyerId = await _db.Contents.Where(q=>q.ContentId == step.Process.RequestId).Select(q=>q.AuthorId).FirstOrDefaultAsync();
                    var buyerWallet = await _db.Wallets.Where(q => q.UserId == buyerId).FirstOrDefaultAsync();
                    if (buyerWallet == null)
                    {
                        return (ProblemDetailTemplate("Buyer wallet doesn't exist"), null);
                    }
                    MidtransStatus? midtransStatus = null;
                    long amountReturnedByWallet = 0;
                    if (step.Transaction != null)
                    {
                        if (step.Transaction.ExternalRef != null)
                        {
                            var status = await _midtransService.GetStatusAsync(step.Transaction.ExternalRef);
                            if (status != null && status.status_code != "404")
                            {
                                midtransStatus = status;
                            }
                        }
                        else
                        {
                            amountReturnedByWallet += step.Transaction.AmountMinor;
                        }
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
                    if (midtransStatus != null)
                    {
                        var grossAmount = (long)Convert.ToDouble(midtransStatus.gross_amount);
                        await _midtransService.CreateMidtransRefundAsync(midtransStatus.transaction_id, grossAmount, "Step was cancelled by seller");
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
                            ExternalRef = midtransStatus.order_id,
                            ReferenceType = "MidtransRefund",
                            Memo = "Refund via Midtrans",
                            //ReferenceId = refundRequest.RefundRequestId
                        };
                        _db.WalletTransactions.Add(snapTransaction);
                    }
                    step.Transaction = null;
                }
                step.Status = request.Status;
            }
            if(request.MinCompleteEstimate != null)
            {
                step.MinCompleteEstimate = (DateTimeOffset)request.MinCompleteEstimate;
            }
            if (request.MaxCompleteEstimate != null)
            {
                step.MaxCompleteEstimate = (DateTimeOffset)request.MaxCompleteEstimate;
            }
            if(request.Images != null)
            {
                var created = new List<ThesisTestAPI.Entities.Image>();
                foreach (var file in request.Images.Where(q => q.Length > 0))
                {
                    var contentType = file.ContentType;
                    await using var stream = file.OpenReadStream();
                    created.Add(new ThesisTestAPI.Entities.Image
                    {
                        ImageId = Guid.NewGuid(),
                        ContentId = step.StepId,
                        ImageName = file.FileName,
                        CreatedAt = DateTimeOffset.Now,
                    });
                    await _blobStorageService.UploadImageFreeAspectAsync(stream, file.FileName, contentType, Enum.BlobContainers.IMAGES);
                }
                _db.Images.AddRange(created);
            }
            await _db.SaveChangesAsync();
            return (null, new StepResponse
            {
                StepId = request.StepId,
            });
        }
    }
}
