using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using ThesisTestAPI.Models.User;
using ThesisTestAPI.Services;
using ThesisTestAPI.Validators.User;

namespace ThesisTestAPI.Controllers
{
    [Route("api/v1")]
    [ApiController]

    public class UserController : ControllerBase
    {
        private readonly UserService _service;
        private readonly IValidator<UploadPfpModel> _pfpValidator;
        private readonly IValidator<UserEditModel> _editValidator;
        private readonly IValidator<UserLoginModel> _loginValidator;
        private readonly IValidator<UserRegisterModel> _registerValidator;
        
        public UserController(
            UserService service,
            IValidator<UserLoginModel> loginValidator,
            IValidator<UserRegisterModel> registerValidator,
            IValidator<UploadPfpModel> pfpValidator,
            IValidator<UserEditModel> editValidator
            )
        {
            _service = service;
            _loginValidator = loginValidator;
            _registerValidator = registerValidator;
            _editValidator = editValidator;
            _pfpValidator = pfpValidator;
        }
        private BadRequestObjectResult Invalid(string detail)
        {
            var problemDetails = new ProblemDetails
            {
                Type = "http://veryCoolAPI.com/errors/invalid-data",
                Title = "Invalid Request Data",
                Detail = detail,
                Instance = HttpContext.Request.Path
            };
            return BadRequest(problemDetails);
        }
        private BadRequestObjectResult InvalidModelState()
        {
            var problemDetails = new ValidationProblemDetails(ModelState)
            {
                Type = "http://veryCoolAPI.com/errors/validation-error",
                Title = "Invalid Request Parameters",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Invalid modelState",
                Instance = HttpContext.Request.Path
            };
            return BadRequest(problemDetails);
        }
        [HttpGet("get-all-users")]
        public async Task<IActionResult> Get()
        {
            var data = await _service.Get();
            if (data.IsNullOrEmpty())
            {
                return Invalid("No users exist");
            }
            return Ok(data);
        }
        [HttpGet("get-user/{UserId}")]
        public async Task<IActionResult>GetById(Guid UserId)
        {
            var data = await _service.Get(UserId);
            if(data == null)
            {
                return Invalid("Invalid user id");
            }
            return Ok(data);
        }
        
        [HttpPost("user-login")]
        public async Task<IActionResult> Login([FromQuery] UserLoginModel request)
        {
            var validation = await _loginValidator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                return Invalid(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));
            }
            var data = await _service.Login(request.Email);
            return Ok(data);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody]RefreshTokenRequest request)
        {
            var data = await _service.RefreshToken(request.RefreshToken);
            if(data == null)
            {
                return Invalid("Expired refresh token");
            }
            return Ok(data);
        }
        [Authorize]
        [HttpGet("user-data")]
        public async Task<IActionResult> GetUserData()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value?? User.FindFirst("UserId")?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                return Invalid("Invalid user id");
            }
            var user = await _service.Get(Guid.Parse(userId));

            return Ok(user);
        }

        [HttpPost("register-user")]
        public async Task<IActionResult> Register([FromBody]UserRegisterModel request)
        {
            if(!ModelState.IsValid)
            {
                return InvalidModelState();
            }
            var validation = await _registerValidator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                return Invalid(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));
            }
            var data = await _service.Register(request);
            return Ok(data);
        }
        
        [HttpPut("edit-user")]
        public async Task<IActionResult> Edit([FromBody] UserEditModel request)
        {
            if (!ModelState.IsValid)
            {
                return InvalidModelState();
            }
            var validation = await _editValidator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                return Invalid(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));
            }
            var data = await _service.Edit(request);
            return Ok(data);
        }

        [HttpPost("upload-pfp")]
        public async Task<IActionResult> UploadPfp(UploadPfpModel request)
        {
            if (!ModelState.IsValid)
            {
                return InvalidModelState();
            }
            var validation = await _pfpValidator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                return Invalid(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));
            }
            var fileName = $"{Guid.NewGuid()}_{request.file.FileName}";
            var contentType = request.file.ContentType;
            using var stream = request.file.OpenReadStream();
            var imageUrl = await _service.UploadPfp(request.UserId, stream, fileName, contentType);
            return Ok(new { imageUrl });
        }
    }
}
