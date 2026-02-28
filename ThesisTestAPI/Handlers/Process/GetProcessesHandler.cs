using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Process;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Process
{
    public class GetProcessesHandler : IRequestHandler<GetProcessesRequest, (ProblemDetails?, PaginatedProcessesResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly BlobStorageService _blobStorageService;
        public GetProcessesHandler(ThesisDbContext db, BlobStorageService blobStorageService)
        {
            _db = db;
            _blobStorageService = blobStorageService;
        }
        public async Task<(ProblemDetails?, PaginatedProcessesResponse?)> Handle(GetProcessesRequest request, CancellationToken cancellationToken)
        {
            var query = _db.Processes
                .Where(p => p.Request.RequestNavigation.AuthorId == request.UserId)
                .OrderByDescending(p => p.CreatedAt);

            var total = await query.CountAsync();

            var processes = await query
                .Skip((request.pageNumber - 1) * request.pageSize)
                .Take(request.pageSize)
                .Select(p => new
                {
                    p.ProcessId,
                    p.Description,
                    p.Status,
                    p.Title,
                    Seller = p.Request.Seller,
                    User = p.Request.RequestNavigation.Author
                })
                .ToListAsync();

            var list = new List<ProcessResponse>();

            foreach (var process in processes)
            {
                var sellerPic = string.IsNullOrEmpty(process.Seller.SellerPicture)
                    ? ""
                    : await _blobStorageService.GetTemporaryImageUrl(
                        process.Seller.SellerPicture,
                        Enum.BlobContainers.SELLERPICTURE);

                var pfp = string.IsNullOrEmpty(process.User.Pfp)
                    ? ""
                    : await _blobStorageService.GetTemporaryImageUrl(
                        process.User.Pfp,
                        Enum.BlobContainers.PFP);

                list.Add(new ProcessResponse
                {
                    ProcessId = process.ProcessId,
                    Description = process.Description,
                    Status = process.Status,
                    Title = process.Title,
                    User = new Models.User.UserResponse
                    {
                        UserId = process.User.UserId,
                        UserName = process.User.UserName,
                        Pfp = pfp
                    },
                    Seller = new Models.Producer.SellerResponse
                    {
                        SellerId = process.Seller.SellerId,
                        SellerName = process.Seller.SellerName,
                        SellerPicture = sellerPic
                    }
                });
            }

            return (null, new PaginatedProcessesResponse
            {
                Total = total,
                Processes = list
            });
        }
    }
}
