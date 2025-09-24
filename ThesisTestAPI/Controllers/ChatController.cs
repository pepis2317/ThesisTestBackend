using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Handlers.Chat;
using ThesisTestAPI.Models.Chat;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Controllers
{
    [ApiController]
    [Route("api/v1/chat")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ChatController(IMediator mediator)
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
        [HttpPost("conversations/send-messages")]
        public async Task<IActionResult> SendMessage(SendMessageRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(Invalid("User id not found in JWT"));
            }
            var result = await _mediator.Send(request);
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }
    }
}
