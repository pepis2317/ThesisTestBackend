using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThesisTestAPI.Models.Conversation;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ThesisTestAPI.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class ConversationController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ConversationController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("create-conversation")]
        public async Task<IActionResult> CreateConversation(CreateConversationRequest request)
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
