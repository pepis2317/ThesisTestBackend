﻿using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThesisTestAPI.Models.User;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.User
{
    public class LoginHandler : IRequestHandler<UserLoginRequest, (ProblemDetails?, LoginResponse?)>
    {
        private readonly UserService _service;
        private readonly IValidator<UserLoginRequest> _validator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LoginHandler(UserService service,  IValidator<UserLoginRequest> validator, IHttpContextAccessor httpContextAccessor)
        {
            _service = service;
            _validator = validator;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<(ProblemDetails?, LoginResponse?)> Handle(UserLoginRequest request, CancellationToken cancellationToken)
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
            var data = await _service.Login(request.Email);
            return (null, data);
        }
    }
}
