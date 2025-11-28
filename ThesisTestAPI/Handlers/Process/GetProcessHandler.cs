using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Process;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Process
{
    public class GetProcessHandler : IRequestHandler<GetProcessRequest, (ProblemDetails?, ProcessResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly BlobStorageService _blobStorageService;
        public GetProcessHandler(ThesisDbContext db, BlobStorageService blobStorageService)
        {
            _db = db;
            _blobStorageService = blobStorageService;
        }
        public async Task<(ProblemDetails?, ProcessResponse?)> Handle(GetProcessRequest request, CancellationToken cancellationToken)
        {
            var process = await _db.Processes.Include(q=>q.Request).ThenInclude(q=>q.Seller).Include(q=>q.Request).ThenInclude(q=>q.RequestNavigation).ThenInclude(q=>q.Author).Where(q => q.ProcessId == request.ProcessId).FirstOrDefaultAsync();
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
            return (null, new ProcessResponse
            {
                ProcessId = request.ProcessId,
                Title = process.Title,
                Description = process.Description,
                Status = process.Status,
                Seller = new Models.Producer.SellerResponse
                {
                    SellerId = seller.SellerId,
                    SellerName = seller.SellerName,
                    SellerPicture = sellerPic,
                },
                User = new Models.User.UserResponse
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    Pfp = pfp
                }
            });
        }
    }
}
