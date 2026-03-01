using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Enum;
using ThesisTestAPI.Models.Process;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Process
{
    public class GetCompleteRequestHandler : IRequestHandler<GetCompleteRequest, (ProblemDetails?, CompleteProcessResponse?)>
    {
        private readonly ProcessService _service;
        public GetCompleteRequestHandler(ProcessService service)
        {
            _service = service;
        }

        public async Task<(ProblemDetails?, CompleteProcessResponse?)> Handle(GetCompleteRequest request, CancellationToken cancellationToken)
        {
            var result = await _service.GetCompleteRequest(request);
            return (null, result);
        }
    }
}
