using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.Drawing.Printing;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Producer;
using ThesisTestAPI.Models.User;

namespace ThesisTestAPI.Services
{
    public class ProducerService
    {
        private readonly ThesisDbContext _db;
        public ProducerService(ThesisDbContext db)
        {
            _db = db;
        }
        public async Task<List<ProducerResponse>> GetAllProducers()
        {
            var producers = await _db.Producers.Select(x => new ProducerResponse() {
                ProducerId = x.ProducerId,
                ProducerName = x.ProducerName,
                OwnerId = x.OwnerId,
                Rating = x.Rating,
                Clients = x.Clients,
                Banner = x.Banner
            }).ToListAsync();
            return producers;
        }
        public async Task<PaginatedProducersResponse> GetAllProducersPaginated(int pageNumber, int pageSize)
        {
            var totalProducers = _db.Producers.Count();
            var producers = await _db.Producers.Skip((pageNumber-1)*pageSize).Take(pageSize).Select(x => new ProducerResponse()
            {
                ProducerId = x.ProducerId,
                ProducerName = x.ProducerName,
                OwnerId = x.OwnerId,
                Rating = x.Rating,
                Clients = x.Clients,
                Banner = x.Banner
            }).ToListAsync();
            return new PaginatedProducersResponse
            {
                total = totalProducers,
                producers = producers
            };

        }
        public async Task<PaginatedProducersResponse> GetProducersFromQuery(string? searchTerm, double? latitude, double? longitude, int pageNumber, int pageSize)
        {
            Point? userLocation = null;

            if (latitude.HasValue && longitude.HasValue)
            {
                userLocation = new Point(longitude.Value, latitude.Value) { SRID = 4326 };
            }

            var query = _db.Producers.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(q => q.ProducerName.ToLower().Contains(searchTerm));
            }

            if (userLocation != null)
            {
                query = query.OrderBy(p => p.Location.Distance(userLocation));
            }

            var totalProducers = await query.CountAsync();
            var producers = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ProducerResponse
                {
                    ProducerId = x.ProducerId,
                    ProducerName = x.ProducerName,
                    OwnerId = x.OwnerId,
                    Rating = x.Rating,
                    Clients = x.Clients,
                    Banner = x.Banner
                })
                .ToListAsync();

            return new PaginatedProducersResponse
            {
                total = totalProducers,
                producers = producers
            };
        }
    }
}
