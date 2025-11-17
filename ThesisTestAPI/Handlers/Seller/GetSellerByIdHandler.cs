using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Producer;
using ThesisTestAPI.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace ThesisTestAPI.Handlers.Seller
{
    public class GetSellerByIdHandler : IRequestHandler<GetSellerRequest, (ProblemDetails?, SellerResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserService _userService;
        private readonly BlobStorageService _blobStorageService;
        public GetSellerByIdHandler(ThesisDbContext db, IHttpContextAccessor httpContextAccessor, UserService userService , BlobStorageService blobStorageService)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
            _blobStorageService = blobStorageService;
        }

        public async Task<(ProblemDetails?, SellerResponse?)> Handle(GetSellerRequest request, CancellationToken cancellationToken)
        {
            var Seller = await _db.Sellers.Where(q => q.SellerId == request.SellerId).FirstOrDefaultAsync();
            if(Seller == null)
            {
                var problemDetails = new ProblemDetails
                {
                    Type = "http://veryCoolAPI.com/errors/invalid-data",
                    Title = "Invalid Request Data",
                    Detail = string.Join("; ", "no Seller with such id exists"),
                    Instance = _httpContextAccessor.HttpContext?.Request.Path
                };
                return (problemDetails, null);
            }
            var owner = await _userService.Get(Seller.OwnerId);
            var picture = await PictureHelper(Seller.SellerPicture, Enum.BlobContainers.SELLERPICTURE);
            var banner = await PictureHelper(Seller.Banner, Enum.BlobContainers.BANNER);
            return (null, new SellerResponse()
            {
                SellerId = Seller.SellerId,
                SellerName = Seller.SellerName,
                Owner = owner,
                SellerPicture = picture,
                Banner = banner,
                Description = Seller.Description,
                CreatedAt = (DateTime) Seller.CreatedAt
            });
        }
        private async Task<string?> PictureHelper(string? fileName, string container)
        {
            string? imageUrl = await _blobStorageService.GetTemporaryImageUrl(fileName, container);
            return imageUrl;
        }
    }
}
