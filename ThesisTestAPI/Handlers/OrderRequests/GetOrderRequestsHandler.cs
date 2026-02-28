using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.OrderRequests;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.OrderRequests
{
    public class
        GetOrderRequestsHandler : IRequestHandler<GetOrderRequests, (ProblemDetails?, PaginatedOrderRequestsResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly BlobStorageService _blobStorageService;

        public GetOrderRequestsHandler(ThesisDbContext db, BlobStorageService blobStorageService)
        {
            _db = db;
            _blobStorageService = blobStorageService;
        }

        public async Task<(ProblemDetails?, PaginatedOrderRequestsResponse?)> Handle(GetOrderRequests request,
            CancellationToken cancellationToken)
        {
            var query = _db.Contents
                .Where(c =>
                    c.Request != null &&
                    (request.IsSeller
                        ? c.Request.Seller.OwnerId == request.UserId
                        : c.AuthorId == request.UserId))
                .OrderByDescending(c => c.CreatedAt);

            var total = await query.CountAsync();

            var data = await query
                .Skip((request.pageNumber - 1) * request.pageSize)
                .Take(request.pageSize)
                .Select(c => new
                {
                    c.CreatedAt,
                    RequestId = c.Request.RequestId,
                    Title = c.Request.RequestTitle,
                    Status = c.Request.RequestStatus,
                    SellerId = c.Request.SellerId,
                    SellerName = c.Request.Seller.SellerName,
                    SellerPicture = c.Request.Seller.SellerPicture,
                    DeclineReason = c.Request.DeclineReason,
                    BuyerUserId = c.Author.UserId,
                    BuyerName = c.Author.UserName,
                    BuyerPicture = c.Author.Pfp
                })
                .ToListAsync();

            var tasks = data.Select(async item =>
            {
                var sellerPicTask = string.IsNullOrEmpty(item.SellerPicture)
                    ? Task.FromResult("")
                    : _blobStorageService.GetTemporaryImageUrl(item.SellerPicture, Enum.BlobContainers.SELLERPICTURE);

                var buyerPicTask = string.IsNullOrEmpty(item.BuyerPicture)
                    ? Task.FromResult("")
                    : _blobStorageService.GetTemporaryImageUrl(item.BuyerPicture, Enum.BlobContainers.PFP);

                await Task.WhenAll(sellerPicTask, buyerPicTask);

                return new OrderRequestResponse
                {
                    RequestId = item.RequestId,
                    Title = item.Title,
                    Status = item.Status,
                    SellerName = item.SellerName,
                    BuyerName = item.BuyerName,
                    BuyerUserId = item.BuyerUserId,
                    SellerId = item.SellerId,
                    SellerPictureUrl = sellerPicTask.Result,
                    BuyerPictureUrl = buyerPicTask.Result,
                    DeclineReason = item.DeclineReason
                };
            });

            var result = await Task.WhenAll(tasks);

            return (null, new PaginatedOrderRequestsResponse
            {
                Total = total,
                Requests = result.ToList()
            });
        }
    }
}