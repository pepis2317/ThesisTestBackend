using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Process;

namespace ThesisTestAPI.Handlers.Process
{
    public class GetProcessHandler : IRequestHandler<GetProcessRequest, (ProblemDetails?, ProcessResponse?)>
    {
        private readonly ThesisDbContext _db;
        public GetProcessHandler(ThesisDbContext db)
        {
            _db = db;
        }
        public async Task<(ProblemDetails?, ProcessResponse?)> Handle(GetProcessRequest request, CancellationToken cancellationToken)
        {
            var process = await _db.Processes.Include(q=>q.Request).ThenInclude(q=>q.Seller).Include(q=>q.Request).ThenInclude(q=>q.RequestNavigation).Where(q => q.ProcessId == request.ProcessId).FirstOrDefaultAsync(); 
            return (null, new ProcessResponse
            {
                ProcessId = request.ProcessId,
                Title = process.Title,
                Description = process.Description,
                Status = process.Status,
                SellerId = process.Request.Seller.SellerId,
                UserId = process.Request.RequestNavigation.AuthorId
            });
        }
    }
}
