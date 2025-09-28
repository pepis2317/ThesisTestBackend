using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThesisTestAPI.Models.Producer;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Seller
{
    public class CreateSellerHandler : IRequestHandler<CreateSellerRequest, (ProblemDetails?, SellerResponse?)>
    {
        private readonly SellerService _service;
        private readonly IValidator<CreateSellerRequest> _validator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CreateSellerHandler(SellerService service, IValidator<CreateSellerRequest> validator, IHttpContextAccessor httpContextAccessor)
        {
            _service = service;
            _validator = validator;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<(ProblemDetails?, SellerResponse?)> Handle(CreateSellerRequest request, CancellationToken cancellationToken)
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
            var data = await _service.CreateSeller(request);
            return (null, data);
        }
    }
}
