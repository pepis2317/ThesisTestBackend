using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Enum;
using ThesisTestAPI.Models.Process;
using ThesisTestAPI.Models.Shipment;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Shipment
{
    public class GetShippableHandler : IRequestHandler<GetShippableRequest, (ProblemDetails?, PaginatedProcessesResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly BlobStorageService _blobStorageService;
        public GetShippableHandler(ThesisDbContext db, BlobStorageService blobStorageService)
        {
            _db = db;
            _blobStorageService = blobStorageService;
        }
        public async Task<(ProblemDetails?, PaginatedProcessesResponse?)> Handle(GetShippableRequest request, CancellationToken cancellationToken)
        {
            var completedProcesses= await _db.Processes
                .Skip((request.pageNumber - 1) * request.pageSize)
                .Include(q=>q.Shipments)
                .Include(q=>q.Request).ThenInclude(q=>q.Seller)
                .Include(q => q.Request).ThenInclude(q => q.RequestNavigation).ThenInclude(q=>q.Author)
                .Where(q=>q.Request.Seller.OwnerId == request.UserId && q.Status == ProcessStatuses.COMPLETED && q.Shipments.Count ==0 )
                .ToListAsync();
            var list = new List<ProcessResponse>();
            foreach (var process in completedProcesses)
            {
                var item = new ProcessResponse
                {
                    ProcessId = process.ProcessId,
                    Description = process.Description,
                    Status = process.Status,
                    Title = process.Title
                };
                if (string.IsNullOrEmpty(process.Request.RequestNavigation.Author.Pfp))
                {
                    item.Picture = await _blobStorageService.GetTemporaryImageUrl(process.Request.RequestNavigation.Author.Pfp, Enum.BlobContainers.PFP);
                }
                list.Add(item);
            }
            var total = await _db.Processes.Where(q => q.Request.Seller.OwnerId == request.UserId && q.Status == ProcessStatuses.COMPLETED && q.Shipments == null).CountAsync();
            return (null, new PaginatedProcessesResponse
            {
                Total = total,
                Processes = list
            });
        }
    }
}
