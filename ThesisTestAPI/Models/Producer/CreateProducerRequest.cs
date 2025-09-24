using MediatR;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;

namespace ThesisTestAPI.Models.Producer
{
    public class CreateProducerRequest : IRequest<(ProblemDetails?, ProducerResponse?)>
    {
        public Guid OwnerId { get; set; }
        public string ProducerName { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
