using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Producer;
using ThesisTestAPI.Models.Seller;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Seller
{
    public class SellersByQueryHandler : IRequestHandler<SellerQuery, (ProblemDetails?, PaginatedSellersResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly IValidator<SellerQuery> _validator;
        private readonly UserService _userService;
        private readonly BlobStorageService _blobStorageService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SellersByQueryHandler(ThesisDbContext db,IValidator<SellerQuery> validator, IHttpContextAccessor httpContextAccessor, UserService userService, BlobStorageService blobStorageService)
        {
            _db = db;
            _validator = validator;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
            _blobStorageService = blobStorageService;
        }

        public async Task<(ProblemDetails?, PaginatedSellersResponse?)> Handle(SellerQuery request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                var problemDetails = new ProblemDetails
                {
                    Type = "http://veryCoolAPI.com/errors/invalid-data",
                    Title = "Invalid Request Data",
                    Detail = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)),
                    Instance = _httpContextAccessor.HttpContext?.Request.Path
                };
                return (problemDetails, null);
            }

            Point? userLocation = null;

            if (request.latitude.HasValue && request.longitude.HasValue)
            {
                userLocation = new Point(request.longitude.Value, request.latitude.Value) { SRID = 4326 };
            }

            var query = _db.Sellers.AsQueryable();

            if (!string.IsNullOrEmpty(request.searchTerm))
            {
                request.searchTerm = request.searchTerm.ToLower();
                query = query.Where(q => q.SellerName.ToLower().Contains(request.searchTerm));
            }

            if (userLocation != null)
            {
                query = query.OrderBy(p => p.Location.Distance(userLocation));
            }

            var totalSellers = await query.CountAsync();
            var Sellers = await query
                .Skip((request.pageNumber - 1) * request.pageSize)
                .Take(request.pageSize)
                .ToListAsync();
            var SellersList = new List<SellerResponse>();
            foreach (var Seller in Sellers)
            {
                var SellerResponse = new SellerResponse
                {
                    SellerId = Seller.SellerId,
                    SellerName = Seller.SellerName
                };
                var owner = await _userService.Get(Seller.OwnerId);
                if(owner != null)
                {
                    SellerResponse.Owner = owner;
                }
                var picture = await PictureHelper(Seller.SellerPicture);
                if (picture != null)
                {
                    SellerResponse.SellerPicture = picture;
                }
                var banner = await BannerHelper(Seller.Banner);
                if(banner != null)
                {
                    SellerResponse.Banner = banner;
                }
                SellersList.Add(SellerResponse);
            }
            return (null, new PaginatedSellersResponse
            {
                Total = totalSellers,
                Sellers = SellersList
            });

        }
        private async Task<string?> PictureHelper(string? fileName)
        {
            string? imageUrl = await _blobStorageService.GetTemporaryImageUrl(fileName, Enum.BlobContainers.SELLERPICTURE);
            return imageUrl;
        }
        private async Task<string?> BannerHelper(string? fileName)
        {
            string? imageUrl = await _blobStorageService.GetTemporaryImageUrl(fileName, Enum.BlobContainers.BANNER);
            return imageUrl;
        }
    }
}
