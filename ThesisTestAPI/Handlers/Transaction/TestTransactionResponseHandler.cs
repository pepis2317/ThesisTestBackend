using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Transaction;
using ThesisTestAPI.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace ThesisTestAPI.Handlers.Transaction
{
    public class TestTransactionResponseHandler : IRequestHandler<TestTransactionRequest, (ProblemDetails?, TransactionResponse?)>
    {
        private readonly MidtransService _midtransService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ThesisDbContext _db;
        public TestTransactionResponseHandler(MidtransService midtransService, ThesisDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _midtransService = midtransService;
            _httpContextAccessor = httpContextAccessor;
            _db = db;
        }
        public async Task<(ProblemDetails?, TransactionResponse?)> Handle(TestTransactionRequest request, CancellationToken cancellationToken)
        {
            if (request.Amount <= 0)
            {
                var problemDetails = new ProblemDetails
                {
                    Type = "http://veryCoolAPI.com/errors/invalid-data",
                    Title = "Invalid Request Data",
                    Detail = "Amount cant be <= 0 broke ahh",
                    Instance = _httpContextAccessor.HttpContext?.Request.Path
                };
                return (problemDetails, null);
            }
            var orderId = $"deposit-{Guid.NewGuid()}";
            var transaction = new WalletTransaction
            {
                TransactionId = Guid.NewGuid(),
                WalletId = request.WalletId,
                AmountMinor = request.Amount,
                Direction = "C",
                SignedAmount = request.Amount,
                Type = "Deposit",
                Status = "Pending",
                CreatedAt = DateTimeOffset.Now,
                IdempotencyKey  = Guid.NewGuid().ToString(),
                ExternalRef = orderId,
                ReferenceType = "MidtransSnap",
                Memo = "Deposit via Midtrans Snap"
            };
            _db.WalletTransactions.Add(transaction);
            await _db.SaveChangesAsync();
            var snap = await _midtransService.CreateSnapTransactionAsync(orderId, request.Amount);
            return (null, new TransactionResponse
            {
                orderId = orderId,
                token = snap.token,
                redirectUrl = snap.redirect_url
            });

        }
    }
}
