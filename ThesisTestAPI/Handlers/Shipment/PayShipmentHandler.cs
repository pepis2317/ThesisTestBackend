using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Enum;
using ThesisTestAPI.Models.Biteship;
using ThesisTestAPI.Models.Shipment;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Shipment
{
    public class PayShipmentHandler : IRequestHandler<PayShipmentRequest, (ProblemDetails?, ShipmentResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly BiteshipService _biteshipService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly MidtransService _midtransService;
        public PayShipmentHandler(ThesisDbContext db, IHttpContextAccessor httpContextAccessor, HttpClient httpClient, MidtransService midtransService, BiteshipService biteshipService)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _midtransService = midtransService;
            _biteshipService = biteshipService;
        }
        private ProblemDetails ProblemDetailTemplate(string detail)
        {
            return new ProblemDetails
            {
                Type = "http://veryCoolAPI.com/errors/invalid-data",
                Title = "Pay shipment error",
                Detail = detail,
                Instance = _httpContextAccessor.HttpContext?.Request.Path
            };
        }
        public async Task<(ProblemDetails?, ShipmentResponse?)> Handle(PayShipmentRequest request, CancellationToken cancellationToken)
        {
            var shipment = await _db.Shipments.Include(q=>q.Process).ThenInclude(q=>q.Request).Where(q => q.ShipmentId == request.ShipmentId).FirstOrDefaultAsync();
            if (shipment == null)
            {
                return (ProblemDetailTemplate("Shipping data doesn't exist"), null);
            }
            var buyerId = await _db.Contents.Where(q => q.ContentId == shipment.Process.Request.RequestId).Select(q => q.AuthorId).FirstOrDefaultAsync();
            var buyerWallet = await _db.Wallets.Where(q => q.UserId == buyerId).FirstOrDefaultAsync();
            if (buyerWallet == null)
            {
                return (ProblemDetailTemplate("Buyer wallet doesn't exist"), null);
            }
            if (request.Method == "Wallet")
            {
                var walletTransaction = new WalletTransaction
                {
                    TransactionId = Guid.NewGuid(),
                    WalletId = buyerWallet.WalletId,
                    AmountMinor = request.Amount,
                    CreatedAt = DateTimeOffset.Now,
                    IdempotencyKey = Guid.NewGuid().ToString(),
                    Type = "Fee",
                    Status = TransactionStatuses.POSTED,
                    PostedAt = DateTime.Now,
                    Direction = "D",
                    SignedAmount = -request.Amount,
                    ReferenceType = "Wallet",
                    Memo = "Shipment payment via wallet"
                };
                buyerWallet.BalanceMinor -= request.Amount;
                shipment.TransactionId = walletTransaction.TransactionId;
                shipment.Status = ShipmentStatuses.PAID;
                shipment.UpdatedAt = DateTime.Now;
                _db.WalletTransactions.Add(walletTransaction);
                var orderResponse = await _biteshipService.CreateOrder(request.ShipmentId, request.OriginNote, request.DestinationNote, request.DeliveryType,request.CourierCompany, request.CourierType, request.OrderNote);
                if(orderResponse == null)
                {
                    return (ProblemDetailTemplate("Problem in creating biteship order"), null);
                }
                shipment.OrderId = orderResponse.Id;
                await _db.SaveChangesAsync();
                return (null, new ShipmentResponse
                {
                    ShipmentId = shipment.ShipmentId
                });
            }
            var orderId = $"fee-{Guid.NewGuid()}";
            var transaction = new WalletTransaction
            {
                TransactionId = Guid.NewGuid(),
                Status = TransactionStatuses.PENDING,
                CreatedAt = DateTimeOffset.Now,
                IdempotencyKey = Guid.NewGuid().ToString(),
                ExternalRef = orderId,
                ReferenceType = "MidtransSnap",
                Memo = "Shipment payment via Midtrans Snap",
                WalletId = buyerWallet.WalletId,
                AmountMinor = request.Amount,
                Type = "Fee",
                Direction = "D",
                SignedAmount = -request.Amount,
            };
            shipment.TransactionId = transaction.TransactionId;
            var preset = new PayShipmentPreset
            {
                PresetId = Guid.NewGuid(),
                TransactionId = transaction.TransactionId,
                Method = request.Method,
                CourierCompany = request.CourierCompany,
                CourierType = request.CourierType,
                DeliveryType = request.DeliveryType,
                OrderNote = request.OrderNote,
                OriginNote = request.OriginNote,
                DestinationNote = request.DestinationNote
            };
            _db.WalletTransactions.Add(transaction);
            _db.PayShipmentPresets.Add(preset);
            await _db.SaveChangesAsync();
            var snap = await _midtransService.CreateSnapTransactionAsync(orderId, request.Amount);
            if (snap == null)
            {
                return (ProblemDetailTemplate("Something went wrong when creating midtrans transaction"), null);
            }
            return (null, new ShipmentResponse
            {
                ShipmentId = shipment.ShipmentId,
                token = snap.token,
                redirectUrl = snap.redirect_url
            });
        }
    }
}
