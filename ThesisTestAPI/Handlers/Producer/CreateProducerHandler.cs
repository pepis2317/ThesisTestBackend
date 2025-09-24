using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThesisTestAPI.Models.Producer;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Producer
{
    public class CreateProducerHandler : IRequestHandler<CreateProducerRequest, (ProblemDetails?, ProducerResponse?)>
    {
        private readonly ProducerService _service;
        private readonly IValidator<CreateProducerRequest> _validator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CreateProducerHandler(ProducerService service, IValidator<CreateProducerRequest> validator, IHttpContextAccessor httpContextAccessor)
        {
            _service = service;
            _validator = validator;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<(ProblemDetails?, ProducerResponse?)> Handle(CreateProducerRequest request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                var problemDetails = new ProblemDetails
                {
                    Type = "http://veryCoolAPI.com/errors/invalid-data",
                    Title = "Invalid Request Data",
                    Detail = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)),
                    Instance = _httpContextAccessor.HttpContext?.Request.Path
                };
                return (problemDetails, null);
            }
            var data = await _service.CreateProducer(request);
            return (null, data);
        }
    }
}
