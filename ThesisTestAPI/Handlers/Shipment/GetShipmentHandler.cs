using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Shipment;

namespace ThesisTestAPI.Handlers.Shipment
{
    public class GetShipmentHandler : IRequestHandler<GetShipmentRequest, (ProblemDetails?, ShipmentResponse?)>
    {
        private readonly ThesisDbContext _db;
        public GetShipmentHandler(ThesisDbContext db)
        {
            _db = db;
        }
        public async Task<(ProblemDetails?, ShipmentResponse?)> Handle(GetShipmentRequest request, CancellationToken cancellationToken)
        {
            var shipment = await _db.Shipments.Where(q => q.ShipmentId == request.ShipmentId).Select(q=>new ShipmentResponse
            {
                ShipmentId = q.ShipmentId,
                Name = q.Name,
                Description = q.Description,
                OrderId = q.OrderId,
                Status = q.Status
            }).FirstOrDefaultAsync();
            return (null, shipment);
        }
    }
}
