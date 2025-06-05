using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace robot_controller_api.Controllers
{
    [ApiController]
    [Route("")]
    [AllowAnonymous]            // ← makes the whole controller open
    public class RootController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetRoot() =>
            Ok("Hello, Robot!");
    }
}
