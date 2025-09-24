using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Image;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Image
{
    public class UploadImageHandler : IRequestHandler<UploadImageRequest, (ProblemDetails?, string?)>
    {
        private readonly ImageService _imageService;
        public UploadImageHandler(ImageService imageService)
        {
            _imageService = imageService;
        }
        public async Task<(ProblemDetails?, string?)> Handle(UploadImageRequest request, CancellationToken cancellationToken)
        {
            var imageUrl = await _imageService.UploadImage(request);
            return (null, imageUrl);
        }
    }
}
