using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThesisTestAPI.Models.Producer;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Producer
{
    public class ProducersByQueryHandler : IRequestHandler<ProducerQuery, (ProblemDetails?, PaginatedProducersResponse?)>
    {
        private readonly ProducerService _service;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ProducersByQueryHandler(ProducerService service, IHttpContextAccessor httpContextAccessor)
        {
            _service = service;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<(ProblemDetails?, PaginatedProducersResponse?)> Handle(ProducerQuery request, CancellationToken cancellationToken)
        {
            var data = await _service.GetProducersFromQuery(request.searchTerm, request.latitude, request.longitude,request.pageNumber, request.pageSize);
            if(data.producers == null)
            {
                var problemDetails = new ProblemDetails
                {
                    Type = "http://veryCoolAPI.com/errors/invalid-data",
                    Title = "Invalid Request Data",
                    Detail = "Returned producers is null",
                    Instance = _httpContextAccessor.HttpContext?.Request.Path
                };
                return (problemDetails, null);
            }
            return (null, data);
        }
    }
}
