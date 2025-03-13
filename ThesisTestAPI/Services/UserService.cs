using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Reflection.Metadata;
using System.Xml.Linq;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.User;

namespace ThesisTestAPI.Services
{
    public class UserService
    {
        private readonly ThesisDbContext _db;
        private readonly BlobStorageService _blobStorageService;
        private readonly IDataProtector _protector;
        public UserService(ThesisDbContext db, BlobStorageService blobStorageService, IDataProtectionProvider provider)
        {
            _db = db;
            _blobStorageService = blobStorageService;
            _protector = provider.CreateProtector("CredentialsProtector");
        }
        public async Task<List<UserModel>> Get()
        {
            var users = await _db.Users.ToListAsync();
            // Step 1: Extract profile picture filenames
            var pfpFilenames = users.Select(u => u.Pfp).ToList();

            // Step 2: Batch fetch profile picture URLs (avoid sequential await inside loop)
            var pfpUrls = await Task.WhenAll(pfpFilenames.Select(PfpHelper));

            // Step 3: Construct UserModel list
            var result = users.Select((user, index) => new UserModel
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                Phone = user.Phone,
                Rating = user.Rating,
                Pfp = pfpUrls[index]
            }).ToList();

            return result;
        }
        public async Task<UserModel?>Get(Guid UserId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(q => q.UserId == UserId);
            if(user == null)
            {
                return null;
            }
            var pfp = await PfpHelper(user.Pfp);
            return new UserModel
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                Phone = user.Phone,
                Rating = user.Rating,
                Pfp = pfp
            };
        }
        public async Task<UserModel> Register(UserRegisterModel request)
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
            return new UserModel
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                Phone = user.Phone,
                Rating = user.Rating,
                Pfp = user.Pfp
            };
        }
        public async Task<UserModel?>Login(string email, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(q=>q.Email == email);
            if(user == null)
            {
                return null;
            }
            if(_protector.Unprotect(user.Password) != password)
            {
                return null;
            }
            var pfp = await PfpHelper(user.Pfp);
            return new UserModel
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                Phone = user.Phone,
                Rating = user.Rating,
                Pfp = pfp
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
            string imageUrl = await _blobStorageService.UploadPfpAsync(imageStream, fileName, contentType, "user-pfps");
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
        public async Task<UserModel?> Edit(UserEditModel request)
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
            
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
            return new UserModel
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                Phone = user.Phone,
                Rating = user.Rating,
                Pfp = user.Pfp
            };
        }
    }
}
