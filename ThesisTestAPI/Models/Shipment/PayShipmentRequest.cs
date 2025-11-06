using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Shipment
{
    public class PayShipmentRequest : IRequest<(ProblemDetails?, ShipmentResponse?)>
    {
        public Guid ShipmentId { get; set; }
        public long Amount { get; set; }
        public string Method { get; set; } = string.Empty;
        public string CourierCompany { get; set; } = string.Empty;
        public string CourierType { get; set; } = string.Empty;
        public string OrderNote { get; set; } = string.Empty;
        public string OriginNote { get; set; } = string.Empty;
        public string DestinationNote { get; set; } = string.Empty;
    }
}
