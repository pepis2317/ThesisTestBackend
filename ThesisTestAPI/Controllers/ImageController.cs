using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThesisTestAPI.Models.Image;
using ThesisTestAPI.Models.Post;

namespace ThesisTestAPI.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ImageController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(UploadImageRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }
        [HttpGet("get-images")]
        public async Task<IActionResult> GetImage([FromQuery] GetImagesRequest request)
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
