using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Notifcations;
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
        private readonly IMediator _mediator;
        
        public UserController( IMediator mediator,UserService service)
        {
            _mediator = mediator;
            _service = service;
        }
        private ProblemDetails Invalid(string details)
        {
            var problemDetails = new ProblemDetails
            {
                Type = "http://veryCoolAPI.com/errors/invalid-data",
                Title = "Invalid Request Data",
                Detail = details,
                Instance = HttpContext.Request.Path
            };
            return problemDetails;
        }

        [HttpGet("get-all-users")]
        public async Task<IActionResult> Get()
        {
            var data = await _service.Get();
            if (data.IsNullOrEmpty())
            {
                return BadRequest(Invalid("No users exist"));
            }
            return Ok(data);
        }
        [HttpGet("get-user/{UserId}")]
        public async Task<IActionResult>GetById(Guid UserId)
        {
            var data = await _service.Get(UserId);
            if(data == null)
            {
                return BadRequest(Invalid("Invalid user id"));
            }
            return Ok(data);
        }
        
        [HttpPost("user-login")]
        public async Task<IActionResult> Login([FromBody]UserLoginRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }
        [Authorize]
        [HttpPut("expo-push-token")]
        public async Task<IActionResult> PutToken([FromBody]UpdateExpoTokenRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(Invalid("User id not found in JWT"));
            }
            request.UserId = Guid.Parse(userId);
            var result = await _mediator.Send(request);
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }
        [Authorize]
        [HttpGet("notifications")]
        public async Task<IActionResult> GetNotifications([FromQuery]GetNotificationsRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(Invalid("User id not found in JWT"));
            }
            request.UserId = Guid.Parse(userId);
            var result = await _mediator.Send(request);
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody]RefreshTokenRequest request)
        {
            var data = await _service.RefreshToken(request.RefreshToken);
            if(data == null)
            {
                return BadRequest(Invalid("Expired refresh token"));
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
                return BadRequest(Invalid("User id not found in JWT"));
            }
            var user = await _service.Get(Guid.Parse(userId));

            return Ok(user);
        }

        [HttpPost("register-user")]
        public async Task<IActionResult> Register([FromBody]UserRegisterRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }
        
        [HttpPut("edit-user")]
        public async Task<IActionResult> Edit([FromBody] UserEditRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }

        [HttpPost("upload-pfp")]
        public async Task<IActionResult> UploadPfp(UploadPfpRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }
    }
}
