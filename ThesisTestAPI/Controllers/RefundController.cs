using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThesisTestAPI.Models.Refunds;

namespace ThesisTestAPI.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class RefundController : ControllerBase
    {
        private readonly IMediator _mediator;
        public RefundController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("create-refund-request")]
        public async Task<IActionResult> CreateRefundRequest(CreateRefundRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }
        [HttpPut("respond-refund-request")]
        public async Task<IActionResult> RespondRefundRequest(RespondRefundRequest request)
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
