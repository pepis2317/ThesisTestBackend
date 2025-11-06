using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Shipment
{
    public class GetShipmentRequest:IRequest<(ProblemDetails?, ShipmentResponse)>
    {
        public Guid ShipmentId {  get; set; }
    }
}
