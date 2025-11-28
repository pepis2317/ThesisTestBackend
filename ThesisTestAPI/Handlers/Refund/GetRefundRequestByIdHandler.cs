using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Refunds;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Refund
{
    public class GetRefundRequestByIdHandler : IRequestHandler<GetRefundRequestById, (ProblemDetails?, RefundResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly BlobStorageService _blobStorageService;
        public GetRefundRequestByIdHandler(ThesisDbContext db, BlobStorageService blobStorageService)
        {
            _db = db;
            _blobStorageService = blobStorageService;
        }
        public async Task<(ProblemDetails?, RefundResponse?)> Handle(GetRefundRequestById request, CancellationToken cancellationToken)
        {
            var refund = await _db.RefundRequests
               .Include(q => q.Process).ThenInclude(q => q.Request).ThenInclude(q => q.RequestNavigation).ThenInclude(q => q.Author)
               .Include(q => q.Process).ThenInclude(q => q.Request).ThenInclude(q => q.Seller)
               .Where(q=>q.RefundRequestId == request.RefundRequestId).FirstOrDefaultAsync();
            if (refund == null)
            {
                return (null, null);
            }
            var pfp = "";
            var sellerPic = "";
            var seller = refund.Process.Request.Seller;
            var user = refund.Process.Request.RequestNavigation.Author;
            if (!string.IsNullOrEmpty(user.Pfp))
            {
                pfp = await _blobStorageService.GetTemporaryImageUrl(user.Pfp, Enum.BlobContainers.PFP);
            }
            if (!string.IsNullOrEmpty(seller.SellerPicture))
            {
                sellerPic = await _blobStorageService.GetTemporaryImageUrl(seller.SellerPicture, Enum.BlobContainers.SELLERPICTURE);
            }
            return (null, new RefundResponse
            {
                RefundId = refund.RefundRequestId,
                ProcessId = refund.ProcessId,
                Message = refund.Message,
                Status = refund.Status,
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
