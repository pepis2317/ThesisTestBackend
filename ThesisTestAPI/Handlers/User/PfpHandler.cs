using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThesisTestAPI.Models.User;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.User
{
    public class PfpHandler : IRequestHandler<UploadPfpRequest, (ProblemDetails?, string?)>
    {
        private readonly UserService _service;
        private readonly IValidator<UploadPfpRequest> _validator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PfpHandler(UserService service, IValidator<UploadPfpRequest> validator, IHttpContextAccessor httpContextAccessor)
        {
            _service = service;
            _validator = validator;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<(ProblemDetails?, string?)> Handle(UploadPfpRequest request, CancellationToken cancellationToken)
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
            var fileName = $"{Guid.NewGuid()}_{request.file.FileName}";
            var contentType = request.file.ContentType;
            using var stream = request.file.OpenReadStream();
            var imageUrl = await _service.UploadPfp(request.UserId, stream, fileName, contentType);
            return (null, imageUrl);
        }
    }
}
