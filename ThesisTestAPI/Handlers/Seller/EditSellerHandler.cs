using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThesisTestAPI.Models.Producer;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Seller
{
    public class EditSellerHandler : IRequestHandler<EditSellerRequest, (ProblemDetails?, SellerResponse?)>
    {
        private readonly SellerService _service;
        private readonly IValidator<EditSellerRequest> _validator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public EditSellerHandler(SellerService service, IValidator<EditSellerRequest> validator, IHttpContextAccessor httpContextAccessor)
        {
            _service = service;
            _validator = validator;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<(ProblemDetails?, SellerResponse?)> Handle(EditSellerRequest request, CancellationToken cancellationToken)
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
            var data = await _service.EditSeller(request);
            return (null, data);
        }
    }
}
