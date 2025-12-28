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
        private readonly NotificationService _notificationService;
        public PayShipmentHandler(ThesisDbContext db, IHttpContextAccessor httpContextAccessor, NotificationService notificationService, HttpClient httpClient, MidtransService midtransService, BiteshipService biteshipService)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _midtransService = midtransService;
            _biteshipService = biteshipService;
            _notificationService = notificationService;
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
            var shipment = await _db.Shipments.Include(q=>q.Transaction).Include(q=>q.Process).ThenInclude(q=>q.Request).ThenInclude(q=>q.Seller).Where(q => q.ShipmentId == request.ShipmentId).FirstOrDefaultAsync();
            if (shipment == null)
            {
                return (ProblemDetailTemplate("Shipping data doesn't exist"), null);
            }
            var buyerId = await _db.Contents.Where(q => q.ContentId == shipment.Process.Request.RequestId).Select(q => q.AuthorId).FirstOrDefaultAsync();
            var buyerWallet = await _db.Wallets.Include(q=>q.User).Where(q => q.UserId == buyerId).FirstOrDefaultAsync();
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
                shipment.OriginNote = request.OriginNote;
                shipment.DestinationNote = request.DestinationNote;
                shipment.OrderNote = request.OrderNote;
                shipment.DestinationNote = request.DestinationNote;
                shipment.CourierType = request.CourierType;
                shipment.CourierCompany = request.CourierCompany;
                _db.WalletTransactions.Add(walletTransaction);
                await _db.SaveChangesAsync();
                return (null, new ShipmentResponse
                {
                    ShipmentId = shipment.ShipmentId,
                    paymentStatus = TransactionStatuses.POSTED
                });
            }

            if (shipment.Transaction != null)
            {
                if (shipment.Transaction.Status == TransactionStatuses.POSTED)
                {
                    return (null, new ShipmentResponse
                    {
                        ShipmentId = shipment.ShipmentId,
                        paymentStatus = TransactionStatuses.POSTED
                    });
                } 
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
            shipment.OriginNote = request.OriginNote;
            shipment.DestinationNote = request.DestinationNote;
            shipment.OrderNote = request.OrderNote;
            shipment.CourierType = request.CourierType;
            shipment.DestinationNote = request.DestinationNote;
            shipment.CourierCompany = request.CourierCompany;
            _db.WalletTransactions.Add(transaction);
            await _db.SaveChangesAsync();
            var snap = await _midtransService.CreateSnapTransactionAsync(orderId, request.Amount, email:buyerWallet.User.Email,firstName:buyerWallet.User.UserName);
            if (snap == null)
            {
                return (ProblemDetailTemplate("Something went wrong when creating midtrans transaction"), null);
            }
            await _notificationService.SendNotification("Shipping fee has been paid by buyer", shipment.Process.Request.Seller.OwnerId);
            return (null, new ShipmentResponse
            {
                ShipmentId = shipment.ShipmentId,
                token = snap.token,
                redirectUrl = snap.redirect_url,
                paymentStatus = TransactionStatuses.PENDING
            });
        }
    }
}
