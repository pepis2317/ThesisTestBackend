using MediatR;
using Microsoft.AspNetCore.Mvc;
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
        [HttpPost("create-process")]
        public async Task<IActionResult>CreateProcess([FromBody] CreateProcessRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }
        [HttpPost("create-complete-request")]
        public async Task<IActionResult> CreateCompleteRequest([FromBody]CreateCompleteProcessRequest request)
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
    }
}
