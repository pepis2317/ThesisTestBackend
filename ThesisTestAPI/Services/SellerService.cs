using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Types;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Drawing.Printing;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Producer;
using ThesisTestAPI.Models.User;

namespace ThesisTestAPI.Services
{
    public class SellerService
    {
        private readonly ThesisDbContext _db;
        private readonly BlobStorageService _blobStorageService;
        private readonly IDataProtector _protector;
        public SellerService(ThesisDbContext db, BlobStorageService blobStorageService, IDataProtectionProvider provider)
        {
            _db = db;
            _blobStorageService = blobStorageService;
            _protector = provider.CreateProtector("CredentialsProtector");
        }
        public async Task<List<SellerResponse>> GetAllSellers()
        {
            var Sellers = await _db.Sellers.Select(x => new SellerResponse() {
                SellerId = x.SellerId,
                SellerName = x.SellerName,
            }).ToListAsync();
            return Sellers;
        }
        public async Task<SellerResponse?>GetSellerFromId(Guid SellerId)
        {
            var data = await _db.Sellers.FirstOrDefaultAsync(q=>q.SellerId == SellerId);
            if (data == null)
            {
                return null;
            }
            return new SellerResponse
            {
                SellerId = data.SellerId,
                SellerName = data.SellerName,
            };
        }
        public async Task<PaginatedSellersResponse> GetAllSellersPaginated(int pageNumber, int pageSize)
        {
            var totalSellers = _db.Sellers.Count();
            var Sellers = await _db.Sellers.Skip((pageNumber-1)*pageSize).Take(pageSize).Select(x => new SellerResponse
            {
                SellerId = x.SellerId,
                SellerName = x.SellerName,
            }).ToListAsync();
            return new PaginatedSellersResponse
            {
                Total = totalSellers,
                Sellers = Sellers
            };

        }

        public async Task<SellerResponse?> CreateSeller(CreateSellerRequest request)
        {
            if (!request.Latitude.HasValue || !request.Longitude.HasValue)
            {
                return null;
            }
            var userId = Guid.NewGuid();
            var user = new User
            {
                UserId = userId,
                UserName = request.UserName,
                Email = request.Email,
                Phone = request.Phone,
                Password = _protector.Protect(request.Password),
                Role = request.Role,
                Address = request.Address,
                PostalCode = request.PostalCode,
                Location = new Point(request.Longitude.Value, request.Latitude.Value) { SRID = 4326 },
            };
            var wallet = new Wallet
            {
                WalletId = Guid.NewGuid(),
                UserId = user.UserId,
                Currency = "IDR",
                BalanceMinor = 0,
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
            };

            _db.Users.Add(user);
            _db.Wallets.Add(wallet);
            var SellerId = Guid.NewGuid();

            var Seller = new Seller
            {
                OwnerId = userId,
                SellerName = request.SellerName,
                SellerId = SellerId,
                Banner = null,
                CreatedAt = DateTime.Now
            };

            _db.Sellers.Add(Seller);
            await _db.SaveChangesAsync();

            return new SellerResponse
            {
                SellerId = Seller.SellerId,
                SellerName = Seller.SellerName,
            };
        }
        public async Task<SellerResponse?> EditSeller(EditSellerRequest request)
        {
            var Seller = await _db.Sellers.FirstOrDefaultAsync(q => q.SellerId == request.SellerId);
            if(Seller == null)
            {
                return null;
            }
            Seller.SellerName = string.IsNullOrEmpty(request.SellerName) ? Seller.SellerName : request.SellerName;
            Seller.Description = string.IsNullOrEmpty(request.Description) ? Seller.Description : request.Description;
            if(request.Latitude.HasValue && request.Longitude.HasValue)
            {
                Seller.Location = new Point(request.Longitude.Value, request.Latitude.Value) { SRID = 4326 };
            }
            if (request.SellerPicture != null)
            {
                var sellerPictureName = $"{Guid.NewGuid()}_{request.SellerPicture.FileName}";
                var contentType = request.SellerPicture.ContentType;
                using var sellerPictureStream = request.SellerPicture.OpenReadStream();
                if (!string.IsNullOrEmpty(Seller.SellerPicture))
                {
                    await _blobStorageService.DeleteFileAsync(Seller.SellerPicture, Enum.BlobContainers.SELLERPICTURE);
                }
                await _blobStorageService.UploadImageAsync(sellerPictureStream, sellerPictureName, contentType, Enum.BlobContainers.SELLERPICTURE, 200);
                Seller.SellerPicture = sellerPictureName;
            }
            if(request.Banner != null)
            {
                var bannerName = $"{Guid.NewGuid()}_{request.Banner.FileName}";
                var contentType = request.Banner.ContentType;
                using var bannerStream = request.Banner.OpenReadStream();
                if (!string.IsNullOrEmpty(Seller.Banner)){
                    await _blobStorageService.DeleteFileAsync(Seller.Banner, Enum.BlobContainers.BANNER);
                }
                await _blobStorageService.UploadImageFreeAspectAsync(bannerStream, bannerName, contentType, Enum.BlobContainers.BANNER);
                Seller.Banner = bannerName;
            }
            _db.Sellers.Update(Seller);
            await _db.SaveChangesAsync();
            return new SellerResponse
            {
                SellerId = Seller.SellerId,
                SellerName = Seller.SellerName,
            };
        }
        public async Task<string?> UploadBanner(Guid SellerId, Stream imageStream, string fileName, string contentType)
        {
            var Seller = await _db.Sellers.FirstOrDefaultAsync(q=>q.SellerId == SellerId);
            if(Seller == null)
            {
                return null;
            }
            if (!string.IsNullOrEmpty(Seller.Banner))
            {   
                await _blobStorageService.DeleteFileAsync(Seller.Banner, Enum.BlobContainers.BANNER);
            }
            string imageUrl = await _blobStorageService.UploadImageAsync(imageStream, fileName, contentType, Enum.BlobContainers.BANNER, 200);
            Seller.Banner = fileName;
            _db.Sellers.Update(Seller);
            await _db.SaveChangesAsync();
            return imageUrl;
        }
    }
}
