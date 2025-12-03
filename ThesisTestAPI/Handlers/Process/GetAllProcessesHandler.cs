using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Process;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Process
{
    public class GetAllProcessesHandler : IRequestHandler<GetAllProcessesRequest, (ProblemDetails?, PaginatedProcessesResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly BlobStorageService _blobStorageService;
        public GetAllProcessesHandler(ThesisDbContext db, BlobStorageService blobStorageService)
        {
            _db = db;
            _blobStorageService = blobStorageService;
        }
        public async Task<(ProblemDetails?, PaginatedProcessesResponse?)> Handle(GetAllProcessesRequest request, CancellationToken cancellationToken)
        {
            var processes = await _db.Processes.Include(q => q.Request).ThenInclude(q => q.Seller).Include(q => q.Request).ThenInclude(q => q.RequestNavigation).ThenInclude(q => q.Author).Skip((request.pageNumber - 1) * request.pageSize).OrderByDescending(q => q.CreatedAt).ToListAsync();
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
                var seller = process.Request.Seller;
                var user = process.Request.RequestNavigation.Author;
                var sellerPic = "";
                var pfp = "";
                if (!string.IsNullOrEmpty(user.Pfp))
                {
                    pfp = await _blobStorageService.GetTemporaryImageUrl(user.Pfp, Enum.BlobContainers.PFP);
                }
                if (!string.IsNullOrEmpty(seller.SellerPicture))
                {
                    sellerPic = await _blobStorageService.GetTemporaryImageUrl(seller.SellerPicture, Enum.BlobContainers.SELLERPICTURE);
                }
                item.User = new Models.User.UserResponse
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    Pfp = pfp
                };
                item.Seller = new Models.Producer.SellerResponse
                {
                    SellerId = seller.SellerId,
                    SellerName = seller.SellerName,
                    SellerPicture = sellerPic,
                };
                list.Add(item);

            }
            var total = await _db.Processes.Include(q => q.Request).ThenInclude(q => q.RequestNavigation).CountAsync();
            return (null, new PaginatedProcessesResponse
            {
                Total = total,
                Processes = list
            });
        }
    }
}
