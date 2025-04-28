using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using ThesisTestAPI.Models.User;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.User
{
    public class RegisterHandler : IRequestHandler<UserRegisterRequest, (ProblemDetails?, UserResponse?)>
    {
        private readonly UserService _service;
        private readonly IValidator<UserRegisterRequest> _validator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RegisterHandler(UserService service, IValidator<UserRegisterRequest> validator, IHttpContextAccessor httpContextAccessor)
        {
            _service = service;
            _validator = validator;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<(ProblemDetails?, UserResponse?)> Handle(UserRegisterRequest request, CancellationToken cancellationToken)
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
            var data = await _service.Register(request);
            return (null, data);
        }
    }
}
