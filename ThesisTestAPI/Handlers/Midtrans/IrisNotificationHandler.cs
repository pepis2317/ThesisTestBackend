using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Enum;
using ThesisTestAPI.Models.Midtrans;
using ThesisTestAPI.Services;
using static System.Net.WebRequestMethods;

namespace ThesisTestAPI.Handlers.Midtrans
{
    public class IrisNotificationHandler : IRequestHandler<IrisPayoutNotification, (ProblemDetails?, string?)>
    {
        private readonly ThesisDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly MidtransService _midtransService;
        public IrisNotificationHandler(ThesisDbContext db, MidtransService midtransService, IHttpContextAccessor httpContextAccessor)
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
        public async Task<(ProblemDetails?, string?)> Handle(IrisPayoutNotification request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.reference_no) || string.IsNullOrWhiteSpace(request.status))
            {
                return (ProblemDetailTemplate("Missing reference_no or status"), null);
            }
            //var status = await _midtransService.GetWithdrawalStatusAsync(request.reference_no);
            //if (status == null)
            //{
            //    return (ProblemDetailTemplate("Unable to verify payout at IRIS"), null);
            //}
            var transaction = await _db.WalletTransactions.Where(q => q.ExternalRef == request.reference_no).FirstOrDefaultAsync();
            if (transaction == null)
            {
                return (ProblemDetailTemplate("Unknown reference_no"), null);
            }
            if(request.status == IrisPayoutNotificationEnum.COMPLETED)
            {
                transaction.Status = TransactionStatuses.POSTED;
                transaction.PostedAt = DateTime.Now;
                var wallet = await _db.Wallets.Where(q=>q.WalletId == transaction.WalletId).FirstOrDefaultAsync();
                if(wallet != null)
                {
                    wallet.BalanceMinor -= (long)Convert.ToDouble(request.amount);
                }
            }
            await _db.SaveChangesAsync();
            return (null, "OK");

        }
    }
}
