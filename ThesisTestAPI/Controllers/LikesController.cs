using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ThesisTestAPI.Models.Like;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ThesisTestAPI.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class LikesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public LikesController(IMediator mediator)
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
        [HttpPost("like-content-test")]
        public async Task<IActionResult> LikeContentTest(LikeRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }
        [Authorize]
        [HttpPost("like-content")]
        public async Task<IActionResult> LikeContent(LikeAuthorizedRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(Invalid("User id not found in JWT"));
            }
            var result = await _mediator.Send(new LikeRequest
            {
                ContentId = request.ContentId,
                AuthorId = Guid.Parse(userId)
            });
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }

        [HttpDelete("unlike-content")]
        public async Task<IActionResult> UnlikeContentTest(UnlikeRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }
        [HttpGet("get-likes")]
        public async Task<IActionResult> GetLikes([FromQuery]GetLikesRequest request)
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
