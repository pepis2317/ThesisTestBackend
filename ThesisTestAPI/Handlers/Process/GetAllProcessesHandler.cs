using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Process;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Process
{
    public class GetAllProcessesHandler : IRequestHandler<GetAllProcessesRequest, (ProblemDetails?, PaginatedProcessesResponse?)>
    {
        private readonly ProcessService _service;
        public GetAllProcessesHandler(ProcessService service)
        {
            _service = service;
        }
        public async Task<(ProblemDetails?, PaginatedProcessesResponse?)> Handle(GetAllProcessesRequest request, CancellationToken cancellationToken)
        {
            var result = await _service.GetAllProcesses(request);
            return (null, result);
        }
    }
}
