using MediatR;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;

namespace ThesisTestAPI.Models.Producer
{
    public class CreateSellerRequest : IRequest<(ProblemDetails?, SellerResponse?)>
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public required string Role { get; set; }
        public int PostalCode { get; set; }
        public required string Address { get; set; }
        public string SellerName { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
