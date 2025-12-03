using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Shipment
{
    public class GetAllShipmentsRequest :IRequest<(ProblemDetails?, PaginatedShipmentResponse?)>
    {
        public int pageSize { get; set; }
        public int pageNumber { get; set; }
    }
}
