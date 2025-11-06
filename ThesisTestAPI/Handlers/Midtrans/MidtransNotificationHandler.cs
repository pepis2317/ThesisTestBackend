using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Enum;
using ThesisTestAPI.Models.Biteship;
using ThesisTestAPI.Models.Midtrans;
using ThesisTestAPI.Models.Shipment;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Midtrans
{
    public class MidtransNotificationHandler : IRequestHandler<MidtransNotification, (ProblemDetails?, string?)>
    {
        private readonly MidtransService _midtransService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ThesisDbContext _db;
        private readonly BiteshipService _biteshipService;
        public MidtransNotificationHandler(BiteshipService biteshipService, MidtransService midtransService, ThesisDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _midtransService = midtransService;
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _biteshipService = biteshipService;
        }
        private ProblemDetails ProblemDetailTemplate(string detail)
        {
            return new ProblemDetails
            {
                Type = "http://veryCoolAPI.com/errors/invalid-data",
                Title = "Webhook error",
                Detail = detail,
                Instance = _httpContextAccessor.HttpContext?.Request.Path
            };
        }
        public async Task<(ProblemDetails?, string?)> Handle(MidtransNotification request, CancellationToken cancellationToken)
        {
            if (request.transaction_status == MidtransStatuses.REFUND || request.transaction_status == MidtransStatuses.PARTIALREFUND)
            {
                var transaction = await _db.WalletTransactions.Where(q => q.ExternalRef == request.order_id && q.Type == "Refund").FirstOrDefaultAsync();
                if (transaction == null)
                {
                    return (ProblemDetailTemplate("Unknown order_id"), null);
                }
                transaction.Status = TransactionStatuses.POSTED;
                transaction.PostedAt = DateTime.Now;
                var wallet = await _db.Wallets.FindAsync(transaction.WalletId);
                if (wallet != null)
                {
                    wallet.BalanceMinor += (long)Convert.ToDouble(request.gross_amount);
                }
            }
            else
            {
                var transaction = await _db.WalletTransactions.Where(q => q.ExternalRef == request.order_id).FirstOrDefaultAsync();
                if (transaction == null)
                {
                    return (ProblemDetailTemplate("Unknown order_id"), null);
                }
                var verify = _midtransService.VerifySignature(request);
                if (verify == false)
                {
                    return (ProblemDetailTemplate("Invalid signature"), null);
                }
                var status = await _midtransService.GetStatusAsync(request.order_id);
                if (status == null)
                {
                    return (ProblemDetailTemplate("Could not fetch status, will retry"), null);
                }
                var isSuccess = status.transaction_status == MidtransStatuses.SETTLEMENT || (status.transaction_status == MidtransStatuses.CAPTURE && status.fraud_status == MidtransStatuses.ACCEPT);
                var isVoided = status.transaction_status == MidtransStatuses.CANCEL || status.transaction_status == MidtransStatuses.EXPIRE;
                var isFailed = status.transaction_status == MidtransStatuses.DENY || status.transaction_status == MidtransStatuses.EXPIRE;
                if (transaction.Status == TransactionStatuses.POSTED || transaction.Status == TransactionStatuses.VOIDED || transaction.Status == TransactionStatuses.FAILED) return (null, "Ok");
                if (isSuccess)
                {
                    transaction.Status = TransactionStatuses.POSTED;
                    transaction.PostedAt = DateTime.Now;

                    if (transaction.Memo == TransactionMemos.MIDTRANSDEPOSIT)
                    {
                        var wallet = await _db.Wallets.Where(q => q.WalletId == transaction.WalletId).FirstOrDefaultAsync();
                        if (wallet != null)
                        {
                            wallet.BalanceMinor += (long)Convert.ToDouble(request.gross_amount);
                        }
                    }
                    else if (transaction.Memo == TransactionMemos.SNAPPAYMENT)
                    {
                        var step = await _db.Steps.Where(q => q.TransactionId == transaction.TransactionId).FirstOrDefaultAsync();
                        if (step != null)
                        {
                            step.Status = StepStatuses.WORKING;
                        }
                    }
                    else if (transaction.Memo == TransactionMemos.SHIPMENT)
                    {
                        var shipment = await _db.Shipments.Include(q => q.Process).ThenInclude(q => q.Request).Where(q => q.TransactionId == transaction.TransactionId).FirstOrDefaultAsync();
                        if (shipment != null)
                        {
                            shipment.Status = ShipmentStatuses.PAID;
                            shipment.UpdatedAt = DateTime.Now;
                            //var preset = await _db.PayShipmentPresets.Where(q => q.TransactionId == transaction.TransactionId).FirstOrDefaultAsync();
                            //if (preset != null)
                            //{
                            //    var orderResponse = await _biteshipService.CreateOrder(shipment.ShipmentId, preset.OriginNote, preset.DestinationNote, preset.DeliveryType, preset.CourierCompany, preset.CourierType, preset.OrderNote);
                            //    if (orderResponse == null)
                            //    {
                            //        return (ProblemDetailTemplate("Problem in creating biteship order"), null);
                            //    }
                            //    shipment.OrderId = orderResponse.Id;
                            //}
                        }
                    }
                }
                else if (isVoided)
                {
                    transaction.Status = TransactionStatuses.VOIDED;
                    transaction.VoidedAt = DateTime.UtcNow;
                }
                else if (isFailed)
                {
                    transaction.Status = TransactionStatuses.FAILED;
                    transaction.VoidedAt = DateTime.UtcNow;
                }
            }
            await _db.SaveChangesAsync();

            return (null, "OK");
        }
    }
}
