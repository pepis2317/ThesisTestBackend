using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Process;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Process
{
    public class CreateProcessHandler : IRequestHandler<CreateProcessRequest, (ProblemDetails?, ProcessResponse?)>
    {
        private readonly ProcessService _service;
        public CreateProcessHandler(ProcessService service)
        {
            _service = service;
        }

        public async Task<(ProblemDetails?, ProcessResponse?)> Handle(CreateProcessRequest request, CancellationToken cancellationToken)
        {
            var processResponse = await _service.CreateProcess(request);
            return (null, processResponse);
        }
    }
}
