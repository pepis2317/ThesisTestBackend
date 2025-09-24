using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly IMediator _mediator;
        public RatingController(IMediator mediator)
        {
            _mediator = mediator;
        }

    }
}
