using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Enum;
using ThesisTestAPI.Models.Steps;

namespace ThesisTestAPI.Handlers.Steps
{
    public class CreateStepHandler : IRequestHandler<CreateStepRequest, (ProblemDetails?, StepResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CreateStepHandler(ThesisDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
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
            var process = await _db.Processes.Where(q=>q.ProcessId == request.ProcessId).FirstOrDefaultAsync();
            if(process != null && process.Status == ProcessStatuses.CREATED)
            {
                process.Status = ProcessStatuses.INPROGRESS;
            }
            await _db.SaveChangesAsync();
            return (null, new StepResponse
            {
                StepId = contentId
            });
        }
    }
}
