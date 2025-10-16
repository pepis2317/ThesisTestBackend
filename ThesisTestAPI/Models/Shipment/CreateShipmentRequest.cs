using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Shipment
{
    public class CreateShipmentRequest : IRequest<(ProblemDetails?, ShipmentResponse?)>
    {
        public Guid ProcessId {  get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public double Weight { get; set; }
        public double Length { get; set; }
    }
}
