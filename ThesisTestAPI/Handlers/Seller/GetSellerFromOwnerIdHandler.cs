using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Producer;
using ThesisTestAPI.Models.Seller;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Seller
{
    public class GetSellerFromOwnerIdHandler : IRequestHandler<GetSellerFromOwnerIdRequest, (ProblemDetails?, SellerResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly BlobStorageService _blobStorageService;
        private readonly UserService _userService;
        public GetSellerFromOwnerIdHandler(ThesisDbContext dB, BlobStorageService blobStorageService, UserService userService)
        {
            _db = dB;
            _blobStorageService = blobStorageService;
            _userService = userService;
        }

        public async Task<(ProblemDetails?, SellerResponse?)> Handle(GetSellerFromOwnerIdRequest request, CancellationToken cancellationToken)
        {
            var Seller = await _db.Sellers.Where(q => q.OwnerId == request.OwnerId).FirstOrDefaultAsync();
            var sellerPicture = "";
            if(!string.IsNullOrEmpty(Seller.SellerPicture))
            {
                sellerPicture = await _blobStorageService.GetTemporaryImageUrl(Seller.SellerPicture, Enum.BlobContainers.SELLERPICTURE);
            }
            var banner = "";
            if (!string.IsNullOrEmpty(Seller.Banner))
            {
                banner = await _blobStorageService.GetTemporaryImageUrl(Seller.Banner, Enum.BlobContainers.BANNER);
            }
            var owner = await _userService.Get(Seller.OwnerId);
            return (null, new SellerResponse
            {
                SellerId = Seller.SellerId,
                SellerName = Seller.SellerName,
                SellerPicture = sellerPicture,
                Owner = owner,
                Banner = banner,
                Description = Seller.Description,
                CreatedAt = (DateTime) Seller.CreatedAt
            });
        }
    }
}
