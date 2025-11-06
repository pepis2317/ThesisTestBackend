using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Process
{
    public class GetProcessesRequest:IRequest<(ProblemDetails?, PaginatedProcessesResponse?)>
    {
        public Guid UserId {  get; set; }
        public int pageSize { get; set; }
        public int pageNumber { get; set; }
    }
}
