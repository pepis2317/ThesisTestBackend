using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Enum;
using ThesisTestAPI.Models.Process;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Process
{
    public class CompleteProcessHandler : IRequestHandler<RespondCompleteProcess, (ProblemDetails?, CompleteProcessResponse?)>
    {
        private readonly ProcessService _processService;
        public CompleteProcessHandler(ProcessService processService)
        {
            _processService = processService;
        }
        public async Task<(ProblemDetails?, CompleteProcessResponse?)> Handle(RespondCompleteProcess request, CancellationToken cancellationToken)
        {
            var response = await _processService.CompleteRequest(request);
            return (null, response);
        }
    }
}
