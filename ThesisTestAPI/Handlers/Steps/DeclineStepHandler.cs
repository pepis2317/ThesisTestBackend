using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Enum;
using ThesisTestAPI.Models.Steps;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Steps
{
    public class DeclineStepHandler : IRequestHandler<DeclineStepRequest, (ProblemDetails?, StepResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly NotificationService _notifService;
        public DeclineStepHandler(ThesisDbContext db, NotificationService notifService)
        {
            _db = db;
            _notifService = notifService;
        }  
        public async Task<(ProblemDetails?, StepResponse?)> Handle(DeclineStepRequest request, CancellationToken cancellationToken)
        {
            var step = await _db.Steps.Include(q=>q.Process).ThenInclude(q=>q.Request).ThenInclude(q=>q.Seller).Where(q => q.StepId == request.StepId).FirstOrDefaultAsync();
            step.Status = StepStatuses.DECLINED;
            await _db.SaveChangesAsync();
            var receiverId = step.Process.Request.Seller.OwnerId;
            await _notifService.SendNotification("Step has been declined", receiverId);
            return (null, new StepResponse
            {
                StepId = step.StepId
            });
        }
    }
}
