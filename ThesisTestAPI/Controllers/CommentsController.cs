using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ThesisTestAPI.Models.Comment;
using ThesisTestAPI.Models.Like;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ThesisTestAPI.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CommentsController(IMediator mediator)
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
        [HttpPost("create-comment-test")]
        public async Task<IActionResult> CreateCommentTest(CreateCommentRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }
        [Authorize]
        [HttpPost("create-comment")]
        public async Task<IActionResult> CreateComment(CreateCommentAuthorizedRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(Invalid("User id not found in JWT"));
            }
            var result = await _mediator.Send(new CreateCommentRequest
            {
                AuthorId = Guid.Parse(userId),
                Comment = request.Comment,
                TargetContentId = request.TargetContentId,
            });
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);

        }
        [HttpPut("edit-comment")]
        public async Task<IActionResult> EditComment(EditCommentRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }
        [HttpDelete("delete-comment")]
        public async Task<IActionResult> DeleteComment(DeleteCommentRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }
        [Authorize]
        [HttpGet("get-comments")]
        public async Task<IActionResult> GetComments([FromQuery]GetCommentsRequest request)
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
