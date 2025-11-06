using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Steps
{
    public class GetStepsRequest:IRequest<(ProblemDetails?, PaginatedStepsResponse?)>
    {
        public Guid ProcessId {  get; set; }
        public int pageSize { get; set; }
        public int pageNumber { get; set; }
    }
}
