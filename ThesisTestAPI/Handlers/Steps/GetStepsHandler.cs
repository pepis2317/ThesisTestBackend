using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Steps;

namespace ThesisTestAPI.Handlers.Steps
{
    public class GetStepsHandler : IRequestHandler<GetStepsRequest, (ProblemDetails?, PaginatedStepsResponse?)>
    {
        private readonly ThesisDbContext _db;

        public GetStepsHandler(ThesisDbContext db)
        {
            _db = db;
        }

        public async Task<(ProblemDetails?, PaginatedStepsResponse?)> Handle(GetStepsRequest request, CancellationToken cancellationToken)
        {
            var query = _db.Steps
                .Where(s => s.ProcessId == request.ProcessId)
                .OrderBy(s => s.CreatedAt);

            var total = await query.CountAsync();

            var steps = await query
                .Skip((request.pageNumber - 1) * request.pageSize)
                .Take(request.pageSize)
                .Select(s => new
                {
                    s.StepId,
                    s.Title,
                    s.Description,
                    s.TransactionId,
                    s.MinCompleteEstimate,
                    s.MaxCompleteEstimate,
                    s.Status,
                    s.Amount,
                    Materials = s.Materials
                        .OrderBy(m => m.CreatedAt)
                        .Select(m => new MaterialModel
                        {
                            MaterialId = m.MaterialId,
                            Cost = m.Cost,
                            Name = m.Name,
                            Quantity = m.Quantity,
                            Supplier = m.Supplier,
                            UnitOfMeasurement = m.UnitOfMeasurement,
                            CreatedAt = m.CreatedAt,
                            UpdatedAt = m.UpdatedAt
                        })
                        .ToList()
                })
                .ToListAsync();

            var result = steps.Select(s => new StepResponse
            {
                StepId = s.StepId,
                Title = s.Title,
                Description = s.Description,
                TransactionId = s.TransactionId?.ToString(),
                MinCompleteEstimate = s.MinCompleteEstimate.ToString("dd/MM/yyyy"),
                MaxCompleteEstimate = s.MaxCompleteEstimate.ToString("dd/MM/yyyy"),
                Status = s.Status,
                Price = s.Amount,
                Materials = s.Materials
            }).ToList();

            return (null, new PaginatedStepsResponse
            {
                Total = total,
                Steps = result
            });
        }
    }
}