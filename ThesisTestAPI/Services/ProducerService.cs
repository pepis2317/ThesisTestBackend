using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Types;
using NetTopologySuite;
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
        private readonly BlobStorageService _blobStorageService;
        public ProducerService(ThesisDbContext db, BlobStorageService blobStorageService)
        {
            _db = db;
            _blobStorageService = blobStorageService;
        }
        public async Task<List<ProducerResponse>> GetAllProducers()
        {
            var producers = await _db.Producers.Select(x => new ProducerResponse() {
                ProducerId = x.ProducerId,
                ProducerName = x.ProducerName,
            }).ToListAsync();
            return producers;
        }
        public async Task<ProducerResponse?>GetProducerFromId(Guid producerId)
        {
            var data = await _db.Producers.FirstOrDefaultAsync(q=>q.ProducerId == producerId);
            if (data == null)
            {
                return null;
            }
            return new ProducerResponse
            {
                ProducerId = data.ProducerId,
                ProducerName = data.ProducerName,
            };
        }
        public async Task<PaginatedProducersResponse> GetAllProducersPaginated(int pageNumber, int pageSize)
        {
            var totalProducers = _db.Producers.Count();
            var producers = await _db.Producers.Skip((pageNumber-1)*pageSize).Take(pageSize).Select(x => new ProducerResponse
            {
                ProducerId = x.ProducerId,
                ProducerName = x.ProducerName,
            }).ToListAsync();
            return new PaginatedProducersResponse
            {
                Total = totalProducers,
                Producers = producers
            };

        }

        public async Task<ProducerResponse?> CreateProducer(CreateProducerRequest request)
        {
            if(!request.Latitude.HasValue || !request.Longitude.HasValue)
            {
                return null;
            }
            var producerId = Guid.NewGuid();


            var producer = new Producer
            {
                OwnerId = request.OwnerId,
                ProducerName = request.Latitude.Value+" "+request.Longitude.Value,
                ProducerId = producerId,
                Location = new Point(request.Longitude.Value, request.Latitude.Value) { SRID = 4326 },
                Banner = null
            };
            _db.Producers.Add(producer);
            await _db.SaveChangesAsync();

            return new ProducerResponse
            {
                ProducerId = producer.ProducerId,
                ProducerName = producer.ProducerName,
            };
        }
        public async Task<ProducerResponse?> EditProducer(EditProducerRequest request)
        {
            var producer = await _db.Producers.FirstOrDefaultAsync(q => q.ProducerId == request.ProducerId);
            if(producer == null)
            {
                return null;
            }
            producer.ProducerName = string.IsNullOrEmpty(request.ProducerName) ? producer.ProducerName : request.ProducerName;
            if(request.Latitude.HasValue && request.Longitude.HasValue)
            {
                producer.Location = new Point(request.Longitude.Value, request.Latitude.Value) { SRID = 4326 };
            }
            _db.Producers.Update(producer);
            await _db.SaveChangesAsync();
            return new ProducerResponse
            {
                ProducerId = producer.ProducerId,
                ProducerName = producer.ProducerName,
            };
        }
        public async Task<string?> UploadBanner(Guid ProducerId, Stream imageStream, string fileName, string contentType)
        {
            var producer = await _db.Producers.FirstOrDefaultAsync(q=>q.ProducerId == ProducerId);
            if(producer == null)
            {
                return null;
            }
            if (!string.IsNullOrEmpty(producer.Banner))
            {   
                await _blobStorageService.DeleteFileAsync(producer.Banner, Enum.BlobContainers.BANNER);
            }
            string imageUrl = await _blobStorageService.UploadImageAsync(imageStream, fileName, contentType, Enum.BlobContainers.BANNER, 200);
            producer.Banner = fileName;
            _db.Producers.Update(producer);
            await _db.SaveChangesAsync();
            return imageUrl;
        }
    }
}
