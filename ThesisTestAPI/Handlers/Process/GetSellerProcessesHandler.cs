using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Index.HPRtree;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Process;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Process
{
    public class GetSellerProcessesHandler : IRequestHandler<GetSellerProcessesRequest, (ProblemDetails?, PaginatedProcessesResponse?)>
    {
        private readonly ProcessService _service;
        public GetSellerProcessesHandler( ProcessService service)
        {
            _service = service;
        }
        public async Task<(ProblemDetails?, PaginatedProcessesResponse?)> Handle(GetSellerProcessesRequest request, CancellationToken cancellationToken)
        {
            var response = await _service.GetSellerProcesses(request);
            return (null, response);
        }
    }
}
