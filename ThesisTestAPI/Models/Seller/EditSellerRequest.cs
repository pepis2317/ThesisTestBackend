using MediatR;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;

namespace ThesisTestAPI.Models.Producer
{
    public class EditSellerRequest : IRequest<(ProblemDetails?, SellerResponse?)>
    {
        public Guid SellerId { get; set; }
        public string? SellerName { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

    }
}
