using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Process;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Process
{
    public class GetProcessHandler : IRequestHandler<GetProcessRequest, (ProblemDetails?, ProcessResponse?)>
    {
        private readonly ProcessService _service;
        public GetProcessHandler(ProcessService service)
        {
            _service = service;
        }
        public async Task<(ProblemDetails?, ProcessResponse?)> Handle(GetProcessRequest request, CancellationToken cancellationToken)
        {
            var response = await _service.GetProcess(request);
            return (null, response);
        }
    }
}
