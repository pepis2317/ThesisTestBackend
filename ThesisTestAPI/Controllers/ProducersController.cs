using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ThesisTestAPI.Models.Producer;
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
    }
}
