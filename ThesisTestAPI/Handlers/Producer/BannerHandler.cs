using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Producer;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Producer
{
    public class BannerHandler : IRequestHandler<BannerRequest, (ProblemDetails?, string?)>
    {
        private readonly ThesisDbContext _db;
        private readonly BlobStorageService _blobStorageService;
        public BannerHandler(ThesisDbContext db, BlobStorageService blobStorageService)
        {
            _db = db;
            _blobStorageService = blobStorageService;
        }
        public async Task<(ProblemDetails?, string?)> Handle(BannerRequest request, CancellationToken cancellationToken)
        {
            var fileName = $"{Guid.NewGuid()}_{request.File.FileName}";
            var contentType = request.File.ContentType;
            using var stream = request.File.OpenReadStream();

            var producer = await _db.Producers.FirstOrDefaultAsync(q => q.ProducerId == request.ProducerId);
            if (producer != null)
            {
                if (!string.IsNullOrEmpty(producer.Banner))
                {
                    await _blobStorageService.DeleteFileAsync(producer.ProducerPicture, Enum.BlobContainers.BANNER);
                }
                string url = await _blobStorageService.UploadImageAsync(stream, fileName, contentType, Enum.BlobContainers.BANNER, 200);
                producer.Banner = url;
                _db.Producers.Update(producer);
                await _db.SaveChangesAsync();
                return (null, url);
            }
            return (null, "failed to upload producer image");
        }
    }
}
