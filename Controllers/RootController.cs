using Microsoft.AspNetCore.Mvc;

namespace RobotControllerApi.Controllers
{
    [ApiController]
    [Route("")]
    public class RootController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetRoot()
        {
            return Ok("Hello, Robot!");
        }
    }
}
