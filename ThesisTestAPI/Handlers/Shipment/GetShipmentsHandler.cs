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
            var processIds = await _db.Processes.Include(q => q.Request).ThenInclude(q => q.RequestNavigation).Where(q => q.Request.RequestNavigation.AuthorId == request.UserId).Select(q=>q.ProcessId).ToListAsync();
            var shipments = await _db.Shipments.Skip((request.pageNumber - 1) * request.pageSize).Where(q => processIds.Contains(q.ProcessId)).OrderByDescending(q=>q.CreatedAt).ToListAsync();
            var list = new List<ShipmentResponse>();
            foreach(var shipment in shipments)
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
