using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Refunds;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Refund
{
    public class GetRefundRequestsHandler : IRequestHandler<GetRefundRequest, (ProblemDetails?, PaginatedRefundRequestResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly BlobStorageService _blobStorageService;
        public GetRefundRequestsHandler(ThesisDbContext db, BlobStorageService blobStorageService)
        {
            _db = db;
            _blobStorageService = blobStorageService;
        }

        public async Task<(ProblemDetails?, PaginatedRefundRequestResponse?)> Handle(GetRefundRequest request, CancellationToken cancellationToken)
        {
            var refunds = await _db.RefundRequests
                .Include(q=>q.Process).ThenInclude(q=>q.Request).ThenInclude(q=>q.RequestNavigation).ThenInclude(q=>q.Author)
                .Include(q => q.Process).ThenInclude(q => q.Request).ThenInclude(q=>q.Seller)
                .Skip((request.pageNumber - 1) * request.pageSize).OrderByDescending(q => q.CreatedAt).ToListAsync();
            var list = new List<RefundResponse>();
            foreach(var refund in refunds)
            {
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
                list.Add(new RefundResponse
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
            var total = await _db.RefundRequests.CountAsync();
            return (null, new PaginatedRefundRequestResponse
            {
                RefundRequests= list,
                Total = total
            });

        }
    }
}
