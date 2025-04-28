using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Xml.Linq;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.User;

namespace ThesisTestAPI.Services
{
    public class UserService
    {
        private readonly ThesisDbContext _db;
        private readonly BlobStorageService _blobStorageService;
        private readonly JwtService _jwtService;
        private readonly IDataProtector _protector;
        public UserService(ThesisDbContext db, BlobStorageService blobStorageService, IDataProtectionProvider provider, JwtService jwtService)
        {
            _db = db;
            _blobStorageService = blobStorageService;
            _protector = provider.CreateProtector("CredentialsProtector");
            _jwtService = jwtService;
        }
        public async Task<List<UserResponse>> Get()
        {
            var users = await _db.Users.ToListAsync();
            var pfpFilenames = users.Select(u => u.Pfp).ToList();
            var pfpUrls = await Task.WhenAll(pfpFilenames.Select(PfpHelper));
            var result = users.Select((user, index) => new UserResponse
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                Phone = user.Phone,
                Rating = user.Rating,
                Pfp = pfpUrls[index],
                Role = user.Role
            }).ToList();

            return result;
        }
        
        public async Task<UserResponse?>Get(Guid UserId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(q => q.UserId == UserId);
            if(user == null)
            {
                return null;
            }
            var pfp = await PfpHelper(user.Pfp);
            return new UserResponse
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                Phone = user.Phone,
                Rating = user.Rating,
                Pfp = pfp,
                Role = user.Role
            };
        }
        public async Task<UserResponse> Register(UserRegisterRequest request)
        {
            var user = new User
            {
                UserId = Guid.NewGuid(),
                UserName = request.UserName,
                Email = request.Email,
                Phone = request.Phone,
                Password = _protector.Protect(request.Password)
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return new UserResponse
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                Phone = user.Phone,
                Rating = user.Rating,
                Pfp = user.Pfp,
                Role = user.Role
            };
        }
        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }
        public async Task<LoginResponse?>Login(string email)
        {
            var user = await _db.Users.FirstOrDefaultAsync(q=>q.Email == email);
            if(user == null)
            {
                return null;
            }
            var token = _jwtService.GenerateToken(user.UserId);
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
            return new LoginResponse
            {
                Token = token,
                RefreshToken = refreshToken
            };
        }
        public async Task<LoginResponse?> RefreshToken(string refreshToken)
        {
            var user = await _db.Users.FirstOrDefaultAsync(q => q.RefreshToken == refreshToken);
            if(user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                return null;
            }
            var newToken = _jwtService.GenerateToken(user.UserId);
            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1);
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
            return new LoginResponse
            {
                Token = newToken,
                RefreshToken = user.RefreshToken
            };
        }
        public async Task<string?> UploadPfp(Guid UserId, Stream imageStream, string fileName, string contentType)
        {
            var user = await _db.Users.FirstOrDefaultAsync(q => q.UserId == UserId);
            if (user == null)
            {
                return null;
            }
            if(!string.IsNullOrEmpty(user.Pfp))
            {
                await _blobStorageService.DeletePfpAsync(user.Pfp, "user-pfps");
            }
            string imageUrl = await _blobStorageService.UploadImageAsync(imageStream, fileName, contentType, "user-pfps", 200);
            user.Pfp = fileName;
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
            return imageUrl;
        }
        private async Task<string?> PfpHelper(string? fileName)
        {
            string? imageUrl = await _blobStorageService.GetTemporaryImageUrl(fileName, "user-pfps");
            return imageUrl;
        }
        public async Task<UserResponse?> Edit(UserEditRequest request)
        {
            var user = await _db.Users.FirstOrDefaultAsync(q=>q.UserId == request.UserId);
            if (user == null)
            {
                return null;
            }
            user.UserName = string.IsNullOrEmpty(request.UserName)?user.UserName:request.UserName;
            user.Email = string.IsNullOrEmpty(request.Email) ? user.Email : request.Email;
            user.Password = string.IsNullOrEmpty(request.Password) ? user.Password : _protector.Protect(request.Password);
            user.Phone = string.IsNullOrEmpty(request.Phone) ? user.Phone : request.Phone;
            user.Role = string.IsNullOrEmpty(request.Role)? user.Role : request.Role;
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
            return new UserResponse
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                Phone = user.Phone,
                Rating = user.Rating,
                Pfp = user.Pfp,
                Role = user.Role
            };
        }
    }
}
