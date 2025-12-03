using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ThesisTestAPI.Models.Process;

namespace ThesisTestAPI.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class ProcessController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProcessController(IMediator mediator)
        {
            _mediator = mediator;
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
        [HttpGet("get-all-processes")]
        public async Task<IActionResult> GetAllProcesses([FromQuery]GetAllProcessesRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }
        [HttpPost("create-process")]
        public async Task<IActionResult> CreateProcess([FromBody] CreateProcessRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }
        [HttpGet("get-complete-request")]
        public async Task<IActionResult> GetCompleteRequest([FromQuery]GetCompleteRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }
        [HttpPost("create-complete-request")]
        public async Task<IActionResult> CreateCompleteRequest([FromBody] CreateCompleteProcessRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }
        [HttpPut("respond-complete-request")]
        public async Task<IActionResult> CompleteProcess([FromBody] RespondCompleteProcess request)
        {
            var result = await _mediator.Send(request);
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }
        [HttpGet("get-process")]
        public async Task<IActionResult> GetProcess([FromQuery] GetProcessRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }
        [Authorize]
        [HttpGet("get-processes")]
        public async Task<IActionResult> GetProcesses([FromQuery] GetProcessesRequest request)
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
        [HttpGet("get-seller-processes")]
        public async Task<IActionResult> GetSellerProcesses([FromQuery] GetSellerProcessesRequest request)
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
    }
}
