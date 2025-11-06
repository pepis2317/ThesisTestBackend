using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Enum;
using ThesisTestAPI.Models.Shipment;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Shipment
{
    public class SendShipmentHandler : IRequestHandler<SendShipmentRequest, (ProblemDetails?, ShipmentResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly BiteshipService _biteshipService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SendShipmentHandler(ThesisDbContext db, BiteshipService biteshipService, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _biteshipService = biteshipService;
            _httpContextAccessor = httpContextAccessor;
        }
        private ProblemDetails ProblemDetailTemplate(string detail)
        {
            return new ProblemDetails
            {
                Type = "http://veryCoolAPI.com/errors/invalid-data",
                Title = "Send shipment error",
                Detail = detail,
                Instance = _httpContextAccessor.HttpContext?.Request.Path
            };
        }
        public async Task<(ProblemDetails?, ShipmentResponse?)> Handle(SendShipmentRequest request, CancellationToken cancellationToken)
        {
            var shipment = await _db.Shipments.Where(q => q.ShipmentId == request.ShipmentId).FirstOrDefaultAsync();
            var orderResponse = await _biteshipService.CreateOrder(shipment.ShipmentId, shipment.OriginNote, shipment.DestinationNote, "now", shipment.CourierCompany, shipment.CourierType, shipment.OrderNote);
            if (orderResponse == null)
            {
                return (ProblemDetailTemplate("Problem in creating biteship order"), null);
            }
            shipment.OrderId = orderResponse.Id;
            shipment.Status = ShipmentStatuses.SENT;
            await _db.SaveChangesAsync();
            return (null, new ShipmentResponse
            {
                ShipmentId = shipment.ShipmentId
            });
        }
    }
}
