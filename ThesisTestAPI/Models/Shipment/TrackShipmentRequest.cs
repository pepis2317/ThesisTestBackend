using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Shipment
{
    public class TrackShipmentRequest : IRequest<(ProblemDetails?, BiteshipTrackResponse? )>
    {
        public string OrderId { get; set; } = string.Empty;
    }
}
