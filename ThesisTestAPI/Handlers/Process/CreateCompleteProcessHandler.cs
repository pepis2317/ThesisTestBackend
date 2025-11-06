using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Enum;
using ThesisTestAPI.Models.Process;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Process
{
    public class CreateCompleteProcessHandler : IRequestHandler<CreateCompleteProcessRequest, (ProblemDetails?, CompleteProcessResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly NotificationService _notifService;
        public CreateCompleteProcessHandler (ThesisDbContext db, NotificationService notifService)
        {
            _db = db;
            _notifService = notifService;
        }
        public async Task<(ProblemDetails?, CompleteProcessResponse?)> Handle(CreateCompleteProcessRequest request, CancellationToken cancellationToken)
        {
            var completeRequest = new CompleteProcessRequest
            {
                RequestId = Guid.NewGuid(),
                ProcessId = request.ProcessId,
                CreatedAt = DateTime.UtcNow,
                Status = RequestStatuses.PENDING,
            };
            var process = await _db.Processes.Include(q=>q.Request).ThenInclude(q=>q.RequestNavigation).Where(q => q.ProcessId == request.ProcessId).FirstOrDefaultAsync();
            _db.CompleteProcessRequests.Add(completeRequest);
            await _db.SaveChangesAsync();
            await _notifService.SendNotification($"Seller wants to complete process {process.Title}", process.Request.RequestNavigation.AuthorId);
            return (null, new CompleteProcessResponse
            {
                CompleteProcessRequestId = request.ProcessId,
                Status = RequestStatuses.PENDING,
            });
        }
    }
}
