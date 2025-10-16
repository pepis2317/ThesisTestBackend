using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Enum;
using ThesisTestAPI.Models.Process;

namespace ThesisTestAPI.Handlers.Process
{
    public class CompleteProcessHandler : IRequestHandler<RespondCompleteProcess, (ProblemDetails?, ProcessResponse?)>
    {
        private readonly ThesisDbContext _db;
        public CompleteProcessHandler(ThesisDbContext db)
        {
            _db = db;
        }
        public async Task<(ProblemDetails?, ProcessResponse?)> Handle(RespondCompleteProcess request, CancellationToken cancellationToken)
        {
            var completeRequest = await _db.CompleteProcessRequests.Where(q => q.RequestId == request.CompleteProcessRequestId).FirstOrDefaultAsync();
            var process = await _db.Processes.Include(q => q.Request).Where(q => q.ProcessId == completeRequest.ProcessId).FirstOrDefaultAsync();
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
            return (null, null);
        }
    }
}
