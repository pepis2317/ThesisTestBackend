using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Steps;

namespace ThesisTestAPI.Handlers.Steps
{
    public class GetStepHandler : IRequestHandler<GetStepRequest, (ProblemDetails?, StepResponse?)>
    {
        private readonly ThesisDbContext _db;
        public GetStepHandler(ThesisDbContext db)
        {
            _db = db;
        }

        public async Task<(ProblemDetails?, StepResponse?)> Handle(GetStepRequest request, CancellationToken cancellationToken)
        {
            var step = await _db.Steps.Where(q => q.StepId == request.StepId).Select(q=>new StepResponse
            {
                StepId = q.StepId,
                Title = q.Title,
                Description = q.Description,
                TransactionId = q.TransactionId == null ? null : q.TransactionId.ToString(),
                MinCompleteEstimate = q.MinCompleteEstimate.ToString("dd/MM/yyyy"),
                MaxCompleteEstimate = q.MaxCompleteEstimate.ToString("dd/MM/yyyy"),
                Status = q.Status
            }).FirstOrDefaultAsync();
            return (null, step);
        }
    }
}
