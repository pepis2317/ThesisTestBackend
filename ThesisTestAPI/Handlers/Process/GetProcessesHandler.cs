using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Process;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Process
{
    public class GetProcessesHandler : IRequestHandler<GetProcessesRequest, (ProblemDetails?, PaginatedProcessesResponse?)>
    {
        private readonly ProcessService _service;
        public GetProcessesHandler(ProcessService service)
        {
            _service = service;
        }
        public async Task<(ProblemDetails?, PaginatedProcessesResponse?)> Handle(GetProcessesRequest request, CancellationToken cancellationToken)
        {
            var result = await _service.GetProcesses(request);

            return (null, result);
        }
    }
}
