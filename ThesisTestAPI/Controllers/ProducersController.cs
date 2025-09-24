using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ThesisTestAPI.Models.Post;
using ThesisTestAPI.Models.Producer;
using ThesisTestAPI.Models.User;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class ProducersController : ControllerBase
    {
        private readonly ProducerService _service;
        private readonly IMediator _mediator;
        public ProducersController(ProducerService service, IMediator mediator)
        {
            _service = service;
            _mediator = mediator;
        }
        [HttpGet("get-all-producers")]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAllProducers();
            return Ok(data);
        }

        [HttpGet("producers-query")]
        public async Task<IActionResult> GetProducersByQuery([FromQuery]ProducerQuery request)
        {
            var result = await _mediator.Send(request);
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }
        [HttpGet("get-producer")]
        public async Task<IActionResult> GetProducerById([FromQuery]GetProducerRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }
        [HttpGet("get-producer-by-owner-id")]
        public async Task<IActionResult> GetProducerByOwnerId([FromQuery]GetProducerFromOwnerIdRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }

        [HttpPost("create-producer")]
        public async Task<IActionResult> CreateProducer([FromBody]CreateProducerRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }
        [HttpPut("edit-producer")]
        public async Task<IActionResult> EditProducer([FromBody]EditProducerRequest request)
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
