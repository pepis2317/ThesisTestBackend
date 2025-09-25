using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Producer;
using ThesisTestAPI.Models.Seller;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Seller
{
    public class SellerPictureHandler : IRequestHandler<UploadSellerPictureRequest, (ProblemDetails?, string?)>
    {
        private readonly ThesisDbContext _db;
        private readonly BlobStorageService _blobStorageService;
        public SellerPictureHandler(ThesisDbContext db, BlobStorageService blobStorageService)
        {
            _db = db;
            _blobStorageService = blobStorageService;
        }
        public async Task<(ProblemDetails?, string?)> Handle(UploadSellerPictureRequest request, CancellationToken cancellationToken)
        {
            var fileName = $"{Guid.NewGuid()}_{request.File.FileName}";
            var contentType = request.File.ContentType;
            using var stream = request.File.OpenReadStream();

            var Seller = await _db.Sellers.FirstOrDefaultAsync(q => q.SellerId == request.SellerId);
            if (Seller != null)
            {
                if (!string.IsNullOrEmpty(Seller.SellerPicture))
                {
                    await _blobStorageService.DeleteFileAsync(Seller.SellerPicture, Enum.BlobContainers.SELLERPICTURE);
                }
                string url = await _blobStorageService.UploadImageAsync(stream, fileName, contentType, Enum.BlobContainers.SELLERPICTURE, 200);
                Seller.SellerPicture = url;
                _db.Sellers.Update(Seller);
                await _db.SaveChangesAsync();
                return (null, url);
            }
            return (null, "failed to upload Seller image");

        }
    }
}
