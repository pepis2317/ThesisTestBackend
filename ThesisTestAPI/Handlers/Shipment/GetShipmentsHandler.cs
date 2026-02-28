using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Shipment;

namespace ThesisTestAPI.Handlers.Shipment
{
    public class GetShipmentsHandler : IRequestHandler<GetShipmentsRequest, (ProblemDetails?, PaginatedShipmentResponse?)>
    {
        private readonly ThesisDbContext _db;
        public GetShipmentsHandler(ThesisDbContext db)
        {
            _db = db;
        }
        public async Task<(ProblemDetails?, PaginatedShipmentResponse?)> Handle(GetShipmentsRequest request, CancellationToken cancellationToken)
        {
            var processIds = await _db.Processes
                .Where(q => q.Request.RequestNavigation.AuthorId == request.UserId)
                .Select(q => q.ProcessId)
                .ToListAsync();

            var query = _db.Shipments
                .Where(q => processIds.Contains(q.ProcessId));

            var total = await query.CountAsync();

            var shipments = await query
                .OrderByDescending(q => q.CreatedAt)
                .Skip((request.pageNumber - 1) * request.pageSize)
                .Take(request.pageSize)
                .Select(shipment => new ShipmentResponse
                {
                    ShipmentId = shipment.ShipmentId,
                    Name = shipment.Name,
                    Description = shipment.Description,
                    Status = shipment.Status,
                    OrderId = shipment.OrderId
                })
                .ToListAsync();

            return (null, new PaginatedShipmentResponse
            {
                Total = total,
                Shipments = shipments
            });
        }
    }
}
