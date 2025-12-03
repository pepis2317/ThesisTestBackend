using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.OrderRequests;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.OrderRequests
{
    public class GetOrderRequestHandler : IRequestHandler<GetOrderRequest, (ProblemDetails?, OrderRequestResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly BlobStorageService _blobStorageService;
        public GetOrderRequestHandler(ThesisDbContext db, BlobStorageService blobStorageService)
        {
            _db = db;
            _blobStorageService = blobStorageService;
        }
        public async Task<(ProblemDetails?, OrderRequestResponse?)> Handle(GetOrderRequest request, CancellationToken cancellationToken)
        {
            var orderRequest = await _db.Requests.Include(q=>q.Seller).Include(q=>q.RequestNavigation).ThenInclude(q=>q.Author).Where(q => q.RequestId == request.RequestId).FirstOrDefaultAsync();
            var order = new OrderRequestResponse
            {
                RequestId = orderRequest.RequestId,
                Title = orderRequest.RequestTitle,
                Status = orderRequest.RequestStatus,
                SellerName = orderRequest.Seller.SellerName,
                BuyerName = orderRequest.RequestNavigation.Author.UserName,
                BuyerUserId = orderRequest.RequestNavigation.Author.UserId,
                SellerId = orderRequest.SellerId,
            };
            if (!string.IsNullOrEmpty(orderRequest.Seller.SellerPicture))
            {
                order.SellerPictureUrl = await _blobStorageService.GetTemporaryImageUrl(orderRequest.Seller.SellerPicture, Enum.BlobContainers.SELLERPICTURE);
            }
            if (!string.IsNullOrEmpty(orderRequest.RequestNavigation.Author.Pfp))
            {
                order.BuyerPictureUrl = await _blobStorageService.GetTemporaryImageUrl(orderRequest.RequestNavigation.Author.Pfp, Enum.BlobContainers.PFP);
            }
            if (!string.IsNullOrEmpty(orderRequest.DeclineReason))
            {
                order.DeclineReason = orderRequest.DeclineReason;
            }
            return (null, order);
        }
    }
}
