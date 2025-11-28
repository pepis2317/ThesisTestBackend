using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.OrderRequests;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.OrderRequests
{
    public class GetOrderRequestsHandler : IRequestHandler<GetOrderRequests, (ProblemDetails?, PaginatedOrderRequestsResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly BlobStorageService _blobStorageService;
        public GetOrderRequestsHandler(ThesisDbContext db, BlobStorageService blobStorageService)
        {
            _db = db;
            _blobStorageService = blobStorageService;
        }
        public async Task<(ProblemDetails?, PaginatedOrderRequestsResponse?)> Handle(GetOrderRequests request, CancellationToken cancellationToken)
        {
            var orderRequests = await _db.Contents
                .Include(c => c.Request).ThenInclude(r => r.Seller)
                .Include(c => c.Author)
                .Skip((request.pageNumber - 1) * request.pageSize)
                .Where(c => (request.IsSeller == true? c.Request.Seller.OwnerId == request.UserId : c.AuthorId == request.UserId )&& c.Request != null)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new{ Request = c.Request, Author = c.Author, CreatedAt = c.CreatedAt}).ToListAsync();
            var result = new List<OrderRequestResponse>();
            foreach (var orderRequest in orderRequests)
            {
                var order = new OrderRequestResponse
                {
                    RequestId = orderRequest.Request.RequestId,
                    Title = orderRequest.Request.RequestTitle,
                    Status = orderRequest.Request.RequestStatus,
                    SellerName = orderRequest.Request.Seller.SellerName,
                    BuyerName = orderRequest.Author.UserName,
                    BuyerUserId = orderRequest.Author.UserId,
                    SellerId = orderRequest.Request.SellerId
                };
                if (!string.IsNullOrEmpty(orderRequest.Request.Seller.SellerPicture))
                {
                    order.SellerPictureUrl = await _blobStorageService.GetTemporaryImageUrl(orderRequest.Request.Seller.SellerPicture, Enum.BlobContainers.SELLERPICTURE);
                }
                if (!string.IsNullOrEmpty(orderRequest.Author.Pfp))
                {
                    order.BuyerPictureUrl = await _blobStorageService.GetTemporaryImageUrl(orderRequest.Author.Pfp, Enum.BlobContainers.PFP);
                }
                result.Add(order);
            }
            var total = await _db.Contents.Include(c => c.Request).ThenInclude(r => r.Seller).Where(c => c.AuthorId == request.UserId && c.Request != null).CountAsync();
            return (null, new PaginatedOrderRequestsResponse
            {
                Total = total,
                Requests = result,
            });
        }
    }
}
