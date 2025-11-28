using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThesisTestAPI.Models.Steps;

namespace ThesisTestAPI.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class StepController : ControllerBase
    {
        private readonly IMediator _mediator;
        public StepController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("create-step")]
        public async Task<IActionResult> CreateStep([FromBody] CreateStepRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }
        [HttpGet("get-steps")]
        public async Task<IActionResult> GetSteps([FromQuery] GetStepsRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }
        [HttpPut("edit-step")]
        public async Task<IActionResult> EditStep( EditStepRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }
        [HttpGet("get-step")]
        public async Task<IActionResult> GetStep([FromQuery] GetStepRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }
        [HttpPut("decline-step")]
        public async Task<IActionResult> Decline([FromBody] DeclineStepRequest request)
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
