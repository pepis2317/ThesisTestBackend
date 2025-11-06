using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Enum;
using ThesisTestAPI.Models.Process;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Process
{
    public class CompleteProcessHandler : IRequestHandler<RespondCompleteProcess, (ProblemDetails?, CompleteProcessResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly NotificationService _notifService;
        public CompleteProcessHandler(ThesisDbContext db, NotificationService notifService)
        {
            _db = db;
            _notifService = notifService;
        }
        public async Task<(ProblemDetails?, CompleteProcessResponse?)> Handle(RespondCompleteProcess request, CancellationToken cancellationToken)
        {
            var completeRequest = await _db.CompleteProcessRequests.Where(q => q.RequestId == request.CompleteProcessRequestId).FirstOrDefaultAsync();
            var process = await _db.Processes.Include(q => q.Request).ThenInclude(q => q.RequestNavigation).ThenInclude(q=>q.Author).Where(q => q.ProcessId == completeRequest.ProcessId).FirstOrDefaultAsync();
            if (request.Response == RequestStatuses.ACCEPTED)
            {
                var ownerId = await _db.Sellers.Where(q => q.SellerId == process.Request.SellerId).Select(q => q.OwnerId).FirstOrDefaultAsync();
                var ownerWallet = await _db.Wallets.Where(q => q.UserId == ownerId).FirstOrDefaultAsync();
                var steps = await _db.Steps.Include(q => q.Transaction).Where(q => q.ProcessId == process.ProcessId).ToListAsync();
                long totalToSend = 0;
                foreach (var step in steps)
                {
                    if (step.Transaction != null)
                    {
                        totalToSend += step.Transaction.AmountMinor;
                    }
                }
                process.Status = ProcessStatuses.COMPLETED;
                ownerWallet.BalanceMinor += totalToSend;
                completeRequest.Status = RequestStatuses.ACCEPTED;
                completeRequest.UpdatedAt = DateTimeOffset.Now;
            }
            else if (request.Response == RequestStatuses.DECLINED)
            {
                completeRequest.Status = RequestStatuses.DECLINED;
                completeRequest.UpdatedAt = DateTimeOffset.Now;
            }
            await _db.SaveChangesAsync();
            var receiverId = completeRequest.Process.Request.RequestNavigation.Author.UserId;
            await _notifService.SendNotification($"Completion request for {process.Title} has been {request.Response}", receiverId);
            return (null, new CompleteProcessResponse { CompleteProcessRequestId = completeRequest.RequestId, Status = completeRequest.Status});
        }
    }
}
