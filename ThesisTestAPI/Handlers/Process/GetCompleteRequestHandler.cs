using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Enum;
using ThesisTestAPI.Models.Process;

namespace ThesisTestAPI.Handlers.Process
{
    public class GetCompleteRequestHandler : IRequestHandler<GetCompleteRequest, (ProblemDetails?, CompleteProcessResponse?)>
    {
        private readonly ThesisDbContext _db;
        public GetCompleteRequestHandler(ThesisDbContext db)
        {
            _db = db;
        }

        public async Task<(ProblemDetails?, CompleteProcessResponse?)> Handle(GetCompleteRequest request, CancellationToken cancellationToken)
        {
            var completeRequest = await _db.CompleteProcessRequests.Where(q => q.ProcessId == request.ProcessId && (q.Status == RequestStatuses.PENDING || q.Status == RequestStatuses.ACCEPTED)).OrderByDescending(q=>q.CreatedAt).FirstOrDefaultAsync();
            if(completeRequest != null)
            {
                return (null, new CompleteProcessResponse
                {
                    CompleteProcessRequestId = completeRequest.RequestId,
                    Status = completeRequest.Status,
                });
            }
            return (null, null);
        }
    }
}
