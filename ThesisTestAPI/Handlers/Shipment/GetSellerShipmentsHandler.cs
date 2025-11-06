using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Process;
using ThesisTestAPI.Models.Shipment;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Shipment
{
    public class GetSellerShipmentsHandler : IRequestHandler<GetSellerShipmentsRequest, (ProblemDetails?, PaginatedShipmentResponse?)>
    {
        private readonly ThesisDbContext _db;
        public GetSellerShipmentsHandler(ThesisDbContext db)
        {
            _db = db;
        }
        public async Task<(ProblemDetails?, PaginatedShipmentResponse?)> Handle(GetSellerShipmentsRequest request, CancellationToken cancellationToken)
        {
            var processIds = await _db.Processes.Include(q => q.Request).ThenInclude(q => q.Seller).Where(q => q.Request.Seller.OwnerId == request.UserId).Select(q => q.ProcessId).ToListAsync();
            var shipments = await _db.Shipments.Skip((request.pageNumber - 1) * request.pageSize).Where(q => processIds.Contains(q.ProcessId)).ToListAsync();
            var list = new List<ShipmentResponse>();
            foreach (var shipment in shipments)
            {
                var item = new ShipmentResponse
                {
                    ShipmentId = shipment.ShipmentId,
                    Name = shipment.Name,
                    Description = shipment.Description,
                    Status = shipment.Status,
                    OrderId = shipment.OrderId
                };
                list.Add(item);
            }
            var total = await _db.Shipments.Where(q => processIds.Contains(q.ProcessId)).CountAsync();
            return (null, new PaginatedShipmentResponse
            {
                Total = total,
                Shipments = list
            });
        }
    }
}
