using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThesisTestAPI.Models.Producer;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Producer
{
    public class EditProducerHandler : IRequestHandler<EditProducerRequest, (ProblemDetails?, ProducerResponse?)>
    {
        private readonly ProducerService _service;
        private readonly IValidator<EditProducerRequest> _validator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public EditProducerHandler(ProducerService service, IValidator<EditProducerRequest> validator, IHttpContextAccessor httpContextAccessor)
        {
            _service = service;
            _validator = validator;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<(ProblemDetails?, ProducerResponse?)> Handle(EditProducerRequest request, CancellationToken cancellationToken)
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
            var data = await _service.EditProducer(request);
            return (null, data);
        }
    }
}
