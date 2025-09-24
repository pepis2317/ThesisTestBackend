using MediatR;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;

namespace ThesisTestAPI.Models.Producer
{
    public class EditProducerRequest : IRequest<(ProblemDetails?, ProducerResponse?)>
    {
        public Guid ProducerId { get; set; }
        public string? ProducerName { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

    }
}
