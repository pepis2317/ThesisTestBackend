using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThesisTestAPI.Models.Process;

namespace ThesisTestAPI.Models.Shipment
{
    public class GetShippableRequest:IRequest<(ProblemDetails?, PaginatedProcessesResponse?)>
    {
        public Guid UserId {  get; set; }
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
    }
}
