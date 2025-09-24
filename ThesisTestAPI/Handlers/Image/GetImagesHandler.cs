using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Image;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Image
{
    public class GetImagesHandler : IRequestHandler<GetImagesRequest, (ProblemDetails?, List<string>?)>
    {
        private readonly ImageService _imageService;
        public GetImagesHandler(ImageService imageService)
        {
            _imageService = imageService;
        }

        public async Task<(ProblemDetails?, List<string>?)> Handle(GetImagesRequest request, CancellationToken cancellationToken)
        {
            var imageUrls = await _imageService.GetImages(request);
            return (null, imageUrls);
        }
    }
}
