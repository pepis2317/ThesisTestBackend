using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Producer;
using ThesisTestAPI.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace ThesisTestAPI.Handlers.Producer
{
    public class GetProducerByIdHandler : IRequestHandler<GetProducerRequest, (ProblemDetails?, ProducerResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserService _userService;
        private readonly BlobStorageService _blobStorageService;
        public GetProducerByIdHandler(ThesisDbContext db, IHttpContextAccessor httpContextAccessor, UserService userService , BlobStorageService blobStorageService)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
            _blobStorageService = blobStorageService;
        }

        public async Task<(ProblemDetails?, ProducerResponse?)> Handle(GetProducerRequest request, CancellationToken cancellationToken)
        {
            var producer = await _db.Producers.Where(q => q.ProducerId == request.ProducerId).FirstOrDefaultAsync();
            if(producer == null)
            {
                var problemDetails = new ProblemDetails
                {
                    Type = "http://veryCoolAPI.com/errors/invalid-data",
                    Title = "Invalid Request Data",
                    Detail = string.Join("; ", "no producer with such id exists"),
                    Instance = _httpContextAccessor.HttpContext?.Request.Path
                };
                return (problemDetails, null);
            }
            var owner = await _userService.Get(producer.OwnerId);
            var picture = await PictureHelper(producer.ProducerPicture);
            return (null, new ProducerResponse()
            {
                ProducerId = producer.ProducerId,
                ProducerName = producer.ProducerName,
                Owner = owner,
                ProducerPicture = picture
            });
        }
        private async Task<string?> PictureHelper(string? fileName)
        {
            string? imageUrl = await _blobStorageService.GetTemporaryImageUrl(fileName, Enum.BlobContainers.PRODUCERPICTURE);
            return imageUrl;
        }
    }
}
