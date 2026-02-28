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
    public class CreateStepHandler : IRequestHandler<CreateStepRequest, (ProblemDetails?, StepResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly NotificationService _notificationService;
        public CreateStepHandler(ThesisDbContext db, IHttpContextAccessor httpContextAccessor, NotificationService notificationService)
        {
            _db = db;
            _notificationService = notificationService;
        }
        public async Task<(ProblemDetails?, StepResponse?)> Handle(CreateStepRequest request, CancellationToken cancellationToken)
        {
            var contentId = Guid.NewGuid();
            var content = new Content
            {
                ContentId = contentId,
                AuthorId = request.AuthorId,
                CreatedAt = DateTimeOffset.Now
            };
            var step = new ThesisTestAPI.Entities.Step
            {
                StepId = contentId,
                ProcessId = request.ProcessId,
                Title = request.Title,
                Description = request.Description,
                MinCompleteEstimate = request.MinCompleteEstimate,
                MaxCompleteEstimate = request.MaxCompleteEstimate,
                Amount = request.Amount,
                Status = StepStatuses.SUBMITTED,
                CreatedAt = DateTimeOffset.Now,
                StepNavigation = content
            };
            if(request.PreviousStepId != null)
            {
                var prev = await _db.Steps.Where(q => q.StepId == request.PreviousStepId).FirstOrDefaultAsync();
                if(prev != null)
                {
                    prev.NextStepId = contentId;
                }
            }
            _db.Steps.Add(step);
            var process = await _db.Processes.Include(q=>q.Request).ThenInclude(q=>q.RequestNavigation).
                Where(q=>q.ProcessId == request.ProcessId).FirstOrDefaultAsync(cancellationToken: cancellationToken);
            if(process != null && process.Status == ProcessStatuses.CREATED)
            {
                process.Status = ProcessStatuses.INPROGRESS;
            }
            
            var materialsList = new List<Material>();
            foreach(var material in request.Materials)
            {
                materialsList.Add(new Material
                {
                    MaterialId = Guid.NewGuid(),
                    StepId = contentId,
                    Name = material.Name,
                    Quantity =  material.Quantity,
                    UnitOfMeasurement =  material.UnitOfMeasurement,
                    Supplier = material.Supplier,
                    Cost = material.Cost,
                    CreatedAt = DateTimeOffset.Now
                });
            }
            _db.Materials.AddRange(materialsList);
            await _db.SaveChangesAsync();
            var receiverId = process.Request.RequestNavigation.AuthorId;
            await _notificationService.SendNotification("Step has been added", receiverId);
            return (null, new StepResponse
            {
                StepId = contentId
            });
        }
    }
}
