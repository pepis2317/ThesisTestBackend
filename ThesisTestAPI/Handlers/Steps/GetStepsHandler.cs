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
            var steps = await _db.Steps
                .Skip((request.pageNumber - 1) * request.pageSize)
                .Where(q => q.ProcessId == request.ProcessId)
                .OrderBy(q => q.CreatedAt)
                .Select(q => new StepResponse
                {
                    StepId = q.StepId,
                    Title = q.Title,
                    Description = q.Description,
                    TransactionId = q.TransactionId == null ? null : q.TransactionId.ToString(),
                    MinCompleteEstimate = q.MinCompleteEstimate.ToString("dd/MM/yyyy"),
                    MaxCompleteEstimate = q.MaxCompleteEstimate.ToString("dd/MM/yyyy"),
                    Status = q.Status,
                    Price = q.Amount,
                    Materials = q.Materials.Select(m => new MaterialModel
                    {
                        MaterialId = m.MaterialId,
                        Cost = m.Cost,
                        Name =  m.Name,
                        Quantity =  m.Quantity,
                        Supplier = m.Supplier,
                        UnitOfMeasurement =  m.UnitOfMeasurement,
                        CreatedAt = m.CreatedAt,
                        UpdatedAt = m.UpdatedAt
                    }).OrderBy(m => m.CreatedAt).ToList()
                }).ToListAsync();
            
            var total = await _db.Steps.Where(q => q.ProcessId == request.ProcessId).CountAsync();
            
            return (null, new PaginatedStepsResponse
            {
                Total = total,
                Steps = steps
            });
        }
    }
}