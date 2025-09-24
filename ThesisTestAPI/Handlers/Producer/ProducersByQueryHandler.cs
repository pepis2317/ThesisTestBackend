using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Producer;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Producer
{
    public class ProducersByQueryHandler : IRequestHandler<ProducerQuery, (ProblemDetails?, PaginatedProducersResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly IValidator<ProducerQuery> _validator;
        private readonly UserService _userService;
        private readonly BlobStorageService _blobStorageService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ProducersByQueryHandler(ThesisDbContext db,IValidator<ProducerQuery> validator, IHttpContextAccessor httpContextAccessor, UserService userService, BlobStorageService blobStorageService)
        {
            _db = db;
            _validator = validator;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
            _blobStorageService = blobStorageService;
        }

        public async Task<(ProblemDetails?, PaginatedProducersResponse?)> Handle(ProducerQuery request, CancellationToken cancellationToken)
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

            var query = _db.Producers.AsQueryable();

            if (!string.IsNullOrEmpty(request.searchTerm))
            {
                request.searchTerm = request.searchTerm.ToLower();
                query = query.Where(q => q.ProducerName.ToLower().Contains(request.searchTerm));
            }

            if (userLocation != null)
            {
                query = query.OrderBy(p => p.Location.Distance(userLocation));
            }

            var totalProducers = await query.CountAsync();
            var producers = await query
                .Skip((request.pageNumber - 1) * request.pageSize)
                .Take(request.pageSize)
                .ToListAsync();
            var producersList = new List<ProducerResponse>();
            foreach (var producer in producers)
            {
                var producerResponse = new ProducerResponse
                {
                    ProducerId = producer.ProducerId,
                    ProducerName = producer.ProducerName
                };
                var owner = await _userService.Get(producer.OwnerId);
                if(owner != null)
                {
                    producerResponse.Owner = owner;
                }
                var picture = await PictureHelper(producer.ProducerPicture);
                if (picture != null)
                {
                    producerResponse.ProducerPicture = picture;
                }
                var banner = await BannerHelper(producer.Banner);
                if(banner != null)
                {
                    producerResponse.Banner = banner;
                }
                producersList.Add(producerResponse);
            }
            return (null, new PaginatedProducersResponse
            {
                Total = totalProducers,
                Producers = producersList
            });

        }
        private async Task<string?> PictureHelper(string? fileName)
        {
            string? imageUrl = await _blobStorageService.GetTemporaryImageUrl(fileName, Enum.BlobContainers.PRODUCERPICTURE);
            return imageUrl;
        }
        private async Task<string?> BannerHelper(string? fileName)
        {
            string? imageUrl = await _blobStorageService.GetTemporaryImageUrl(fileName, Enum.BlobContainers.BANNER);
            return imageUrl;
        }
    }
}
