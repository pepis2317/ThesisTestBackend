using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Controllers
{
    [Route("api/v1")]
    [ApiController]

    public class UserController : ControllerBase
    {
        private readonly UserService _service;
        public UserController(UserService service)
        {
            _service = service;
        }
        [HttpGet("get-all--users")]
        public async Task<IActionResult> Get()
        {
            var data = await _service.Get();
            if (data.IsNullOrEmpty())
            {
                return BadRequest("No users were found");
            }
            return Ok(data);
        }
    }
}
