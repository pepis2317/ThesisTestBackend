using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Drawing.Imaging;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.User;

namespace ThesisTestAPI.Validators.User
{
    public class UploadPfpValidator:AbstractValidator<UploadPfpModel>
    {
        private readonly ThesisDbContext _db;
        public UploadPfpValidator(ThesisDbContext db)
        {
            _db = db;
            RuleFor(x => x.UserId).NotNull().NotEmpty().WithMessage("User id must be provided");
            RuleFor(x => x.UserId).MustAsync(ValidUserId).WithMessage("Invalid user id");
            RuleFor(x => x.file).NotNull().NotEmpty().WithMessage("Image file must be provided");
            RuleFor(x => x.file).Must(ValidImage).WithMessage("Invalid file format");
        }
        private async Task<bool> ValidUserId(Guid UserId, CancellationToken token)
        {
            var user = await  _db.Users.FirstOrDefaultAsync(u => u.UserId == UserId, token);
            if(user == null)
            {
                return false;
            }
            return true;
        }

        private bool ValidImage(IFormFile file)
        {
            long maxSize = 2 * 1024 * 1024;
            if (file.Length > maxSize)
            {
                return false;
            }
            var permittedMimeTypes = new HashSet<string>
            {
                "image/jpeg", "image/png", "image/bmp", "image/webp"
            };
            if (!permittedMimeTypes.Contains(file.ContentType.ToLower()))
            {
                return false;
            }

            var permittedExtensions = new HashSet<string> { ".jpg", ".jpeg", ".png", ".bmp", ".webp" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            if (!permittedExtensions.Contains(fileExtension))
            {
                return false;
            }

            try
            {
                using var image = Image.FromStream(file.OpenReadStream());
                return image.RawFormat.Equals(ImageFormat.Jpeg) ||
                       image.RawFormat.Equals(ImageFormat.Png) ||
                       image.RawFormat.Equals(ImageFormat.Bmp) ||
                       image.RawFormat.Equals(ImageFormat.Webp);
            }
            catch
            {
                return false; // File is not a valid image
            }

        }
    }
}
