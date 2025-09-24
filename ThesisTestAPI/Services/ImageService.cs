using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Image;

namespace ThesisTestAPI.Services
{
    public class ImageService
    {
        private readonly ThesisDbContext _db;
        private readonly BlobStorageService _blobStorageService;
        public ImageService(ThesisDbContext db, BlobStorageService blobStorageService)
        {
            _db = db;
            _blobStorageService = blobStorageService;
        }
        public async Task<string> UploadImage(UploadImageRequest request)
        {
            var fileName = $"{Guid.NewGuid()}_{request.File.FileName}";
            var contentType = request.File.ContentType;
            using var stream = request.File.OpenReadStream();
            var imageUrl = "";
            var content = await _db.Contents.FirstOrDefaultAsync(q => q.ContentId == request.ContentId);
            if (content != null)
            {
                string url = await _blobStorageService.UploadImageAsync(stream, fileName, contentType, Enum.BlobContainers.IMAGES, 200);
                imageUrl = url;
            }
            var image = new ThesisTestAPI.Entities.Image
            {
                ImageId = Guid.NewGuid(),
                ContentId = request.ContentId,
                ImageName = fileName,
                CreatedAt = DateTimeOffset.Now,
            };
            _db.Images.Add(image);
            await _db.SaveChangesAsync();
            return imageUrl;
        }
        public async Task <List<string>>  GetImages(GetImagesRequest request)
        {
            var images = await _db.Images.OrderBy(q => q.CreatedAt).Where(q => q.ContentId == request.ContentId).ToListAsync();
            var imageUrls = new List<string>();
            foreach (var image in images)
            {
                string? imageUrl = await _blobStorageService.GetTemporaryImageUrl(image.ImageName, Enum.BlobContainers.IMAGES);
                if (imageUrl != null)
                {
                    imageUrls.Add(imageUrl);
                }
            }
            return imageUrls;
        }
    }
}
