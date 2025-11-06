using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Index.HPRtree;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Process;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Process
{
    public class GetSellerProcessesHandler : IRequestHandler<GetSellerProcessesRequest, (ProblemDetails?, PaginatedProcessesResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly BlobStorageService _blobStorageService;
        public GetSellerProcessesHandler( ThesisDbContext db, BlobStorageService blobStorageService)
        {
            _db = db;
            _blobStorageService = blobStorageService;
        }
        public async Task<(ProblemDetails?, PaginatedProcessesResponse?)> Handle(GetSellerProcessesRequest request, CancellationToken cancellationToken)
        {
            var orderRequestIds = await _db.Requests.Include(q => q.Seller).Where(q => q.Seller.OwnerId == request.UserId).Select(q => q.RequestId).ToListAsync();
            var processes = await _db.Processes.Skip((request.pageNumber - 1) * request.pageSize).Include(q=>q.Request).ThenInclude(q=>q.RequestNavigation).ThenInclude(q=>q.Author).Where(q => orderRequestIds.Contains(q.RequestId)).OrderByDescending(q => q.CreatedAt).ToListAsync();
            var list = new List<ProcessResponse>();
            foreach (var process in processes)
            {
                var item = new ProcessResponse
                {
                    ProcessId = process.ProcessId,
                    Description = process.Description,
                    Status = process.Status,
                    Title = process.Title
                };
                if (!string.IsNullOrEmpty(process.Request.RequestNavigation.Author.Pfp))
                {
                    item.Picture = await _blobStorageService.GetTemporaryImageUrl(process.Request.RequestNavigation.Author.Pfp, Enum.BlobContainers.PFP);
                }
                list.Add(item);
            }
            var total = await _db.Processes.Where(q => orderRequestIds.Contains(q.RequestId)).CountAsync();
            return (null, new PaginatedProcessesResponse
            {
                Total = total,
                Processes = list
            });
        }
    }
}
