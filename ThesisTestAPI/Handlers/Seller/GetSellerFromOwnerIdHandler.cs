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
        public GetSellerFromOwnerIdHandler(ThesisDbContext dB, BlobStorageService blobStorageService)
        {
            _db = dB;
            _blobStorageService = blobStorageService;
        }

        public async Task<(ProblemDetails?, SellerResponse?)> Handle(GetSellerFromOwnerIdRequest request, CancellationToken cancellationToken)
        {
            var Seller = await _db.Sellers.Where(q => q.OwnerId == request.OwnerId).FirstOrDefaultAsync();
            var pfp = "";
            if(Seller.SellerPicture!= null)
            {
                pfp = await _blobStorageService.GetTemporaryImageUrl(Seller.SellerPicture, Enum.BlobContainers.SELLERPICTURE);
            }
            return (null, new SellerResponse
            {
                SellerId = Seller.SellerId,
                SellerName = Seller.SellerName,
                SellerPicture = Seller.SellerPicture
            });
        }
    }
}
