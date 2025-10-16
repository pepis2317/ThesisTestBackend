using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThesisTestAPI.Models.Shipment;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Shipment
{
    public class TrackShipmentHandler : IRequestHandler<TrackShipmentRequest, (ProblemDetails?, BiteshipTrackResponse?)>
    {
        private readonly BiteshipService _biteshipService;
        public TrackShipmentHandler(BiteshipService biteshipService)
        {
            _biteshipService = biteshipService;
        }

        public async Task<(ProblemDetails?, BiteshipTrackResponse?)> Handle(TrackShipmentRequest request, CancellationToken cancellationToken)
        {
            var trackResponse = await _biteshipService.Track(request.OrderId);
            return (null, trackResponse);
        }
    }
}
