using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ThesisTestAPI.Models.Post;
using ThesisTestAPI.Models.Producer;
using ThesisTestAPI.Models.Seller;
using ThesisTestAPI.Models.User;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class SellersController : ControllerBase
    {
        private readonly SellerService _service;
        private readonly IMediator _mediator;
        public SellersController(SellerService service, IMediator mediator)
        {
            _service = service;
            _mediator = mediator;
        }
        [HttpGet("get-all-sellers")]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAllSellers();
            return Ok(data);
        }

        [HttpGet("sellers-query")]
        public async Task<IActionResult> GetProducersByQuery([FromQuery]SellerQuery request)
        {
            var result = await _mediator.Send(request);
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }
        [HttpGet("get-seller")]
        public async Task<IActionResult> GetProducerById([FromQuery]GetSellerRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }
        [HttpGet("get-seller-by-owner-id")]
        public async Task<IActionResult> GetProducerByOwnerId([FromQuery]GetSellerFromOwnerIdRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }

        [HttpPost("create-seller")]
        public async Task<IActionResult> CreateProducer([FromBody]CreateSellerRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.Item1 != null)
            {
                return BadRequest(result.Item1);
            }
            return Ok(result.Item2);
        }
        [HttpPut("edit-seller")]
        public async Task<IActionResult> EditProducer([FromBody]EditSellerRequest request)
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
