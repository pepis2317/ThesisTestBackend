using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Enum;
using ThesisTestAPI.Models.Process;

namespace ThesisTestAPI.Handlers.Process
{
    public class CreateCompleteProcessHandler : IRequestHandler<CreateCompleteProcessRequest, (ProblemDetails?, ProcessResponse?)>
    {
        private readonly ThesisDbContext _db;
        public CreateCompleteProcessHandler (ThesisDbContext db)
        {
            _db = db;
        }
        public async Task<(ProblemDetails?, ProcessResponse?)> Handle(CreateCompleteProcessRequest request, CancellationToken cancellationToken)
        {
            var completeRequest = new CompleteProcessRequest
            {
                RequestId = Guid.NewGuid(),
                ProcessId = request.ProcessId,
                CreatedAt = DateTime.UtcNow,
                Status = RequestStatuses.PENDING,
            };
            _db.CompleteProcessRequests.Add(completeRequest);
            await _db.SaveChangesAsync();
            return (null, new ProcessResponse
            {
                ProcessId = request.ProcessId,
            });
        }
    }
}
