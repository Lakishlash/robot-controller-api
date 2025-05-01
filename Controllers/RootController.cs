using Microsoft.AspNetCore.Mvc;

namespace robot_controller_api.Controllers
{
    [ApiController]
    [Route("")]
    public class RootController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetRoot() => Ok("Hello, Robot!");
    }
}
