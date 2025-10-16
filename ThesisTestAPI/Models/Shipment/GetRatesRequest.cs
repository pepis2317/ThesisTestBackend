using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Shipment
{
    public class GetRatesRequest : IRequest<(ProblemDetails?, BiteshipRatesResponse?)>
    {
        public Guid ShipmentId {  get; set; }
        public string Couriers {  get; set; } = string.Empty;
    }
}
