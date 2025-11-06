using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.OrderRequests;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.OrderRequests
{
    public class GetSellerOrderRequestsHandler : IRequestHandler<GetSellerOrderRequests, (ProblemDetails?, PaginatedOrderRequestsResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly BlobStorageService _blobStorageService;

        public GetSellerOrderRequestsHandler(ThesisDbContext db, BlobStorageService blobStorageService)
        {
            _db = db;
            _blobStorageService = blobStorageService;
        }
        public async Task<(ProblemDetails?, PaginatedOrderRequestsResponse?)> Handle(GetSellerOrderRequests request, CancellationToken cancellationToken)
        {
            var orderRequests = await _db.Contents
                .Include(c => c.Request)
                .Include(c => c.Author)
                .Skip((request.pageNumber - 1) * request.pageSize)
                .Where(c => c.Request != null && c.Request.Seller.OwnerId == request.UserId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
            var result = new List<OrderRequestResponse>();
            foreach (var orderRequest in orderRequests)
            {
                var order = new OrderRequestResponse
                {
                    RequestId = orderRequest.Request.RequestId,
                    Title = orderRequest.Request.RequestTitle,
                    Status = orderRequest.Request.RequestStatus,
                    Name = orderRequest.Author.UserName
                };
                if (!string.IsNullOrEmpty(orderRequest.Author.Pfp))
                {
                    order.PictureUrl = await _blobStorageService.GetTemporaryImageUrl(orderRequest.Author.Pfp, Enum.BlobContainers.PFP);
                }
                result.Add(order);
            }
            var total = await _db.Contents.Include(c => c.Request).Where(c => c.Request != null && c.Request.Seller.OwnerId == request.UserId).CountAsync();
            return (null, new PaginatedOrderRequestsResponse
            {
                Total = total,
                Requests = result,
            });
        }
    }
}
