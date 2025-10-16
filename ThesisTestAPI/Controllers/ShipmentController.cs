using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThesisTestAPI.Models.Shipment;

namespace ThesisTestAPI.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class ShipmentController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ShipmentController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("create-shipment")]
        public async Task<IActionResult> CreateShipment([FromBody]CreateShipmentRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }
        [HttpPost("pay-shipment")]
        public async Task<IActionResult> PayShipment([FromBody]PayShipmentRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }
        [HttpGet("get-rates")]
        public async Task<IActionResult> GetRates([FromQuery] GetRatesRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }
        [HttpGet("track-order")]
        public async Task<IActionResult> Track([FromQuery]TrackShipmentRequest request)
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
