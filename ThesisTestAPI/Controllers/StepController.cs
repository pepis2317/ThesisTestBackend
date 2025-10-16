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
    }
}
