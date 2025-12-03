using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Process
{
    public class GetAllProcessesRequest:IRequest<(ProblemDetails?, PaginatedProcessesResponse?)>
    {
        public int pageSize { get; set; }
        public int pageNumber { get; set; }
    }
}
