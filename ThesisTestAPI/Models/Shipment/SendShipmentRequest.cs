using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Shipment
{
    public class SendShipmentRequest:IRequest<(ProblemDetails?, ShipmentResponse?)>
    {
        public Guid ShipmentId {  get; set; }
    }
}
