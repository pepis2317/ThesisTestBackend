using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Shipment
{
    public class GetSellerShipmentsRequest:IRequest<(ProblemDetails?, PaginatedShipmentResponse?)>
    {
        public Guid UserId { get; set; }
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
    }
}
