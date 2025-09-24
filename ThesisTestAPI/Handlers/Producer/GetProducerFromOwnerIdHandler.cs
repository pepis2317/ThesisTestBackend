using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Post;
using ThesisTestAPI.Models.Producer;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Producer
{
    public class GetProducerFromOwnerIdHandler : IRequestHandler<GetProducerFromOwnerIdRequest, (ProblemDetails?, ProducerResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly BlobStorageService _blobStorageService;
        public GetProducerFromOwnerIdHandler(ThesisDbContext dB, BlobStorageService blobStorageService)
        {
            _db = dB;
            _blobStorageService = blobStorageService;
        }

        public async Task<(ProblemDetails?, ProducerResponse?)> Handle(GetProducerFromOwnerIdRequest request, CancellationToken cancellationToken)
        {
            var producer = await _db.Producers.Where(q => q.OwnerId == request.OwnerId).FirstOrDefaultAsync();
            var pfp = "";
            if(producer.ProducerPicture!= null)
            {
                pfp = await _blobStorageService.GetTemporaryImageUrl(producer.ProducerPicture, Enum.BlobContainers.PRODUCERPICTURE);
            }
            return (null, new ProducerResponse
            {
                ProducerId = producer.ProducerId,
                ProducerName = producer.ProducerName,
                ProducerPicture = producer.ProducerPicture
            });
        }
    }
}
