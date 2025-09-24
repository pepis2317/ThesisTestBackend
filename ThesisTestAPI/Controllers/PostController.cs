using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ThesisTestAPI.Models.Post;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ThesisTestAPI.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PostController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("get-posts")]
        public async Task<IActionResult> GetPosts([FromQuery]PostQuery query)
        {
            var result = await _mediator.Send(query);
            if(result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }
        [HttpPost("create-post-metadata")]
        public async Task<IActionResult> CreatePostMetadata([FromBody]CreatePostRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }
        [HttpGet("get-cursor-posts")]
        public async Task<IActionResult> GetCursorPosts([FromQuery]GetCursorPostRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }
        [HttpGet("get-post")]
        public async Task<IActionResult> GetPost([FromQuery]GetPostRequest request)
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
