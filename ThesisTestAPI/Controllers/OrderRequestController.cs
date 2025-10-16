using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThesisTestAPI.Models.OrderRequests;

namespace ThesisTestAPI.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class OrderRequestController : ControllerBase
    {
        private readonly IMediator _mediator;
        public OrderRequestController(IMediator mediator) {
            _mediator = mediator;
        }
        [HttpPost("create-order-request")]
        public async Task<IActionResult> CreateOrderRequest([FromBody] CreateOrderRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }
        [HttpPut("respond-order-request")]
        public async Task<IActionResult> RespondOrderRequest([FromBody] RespondOrderRequest request)
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
