using Microsoft.AspNetCore.Mvc;
using robot_controller_api.Models;
using robot_controller_api.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;

namespace robot_controller_api.Controllers
{
    [ApiController]
    [Route("api/robot-commands")]
    public class RobotCommandsController : ControllerBase
    {
        private readonly IRobotCommandDataAccess _repo;

        public RobotCommandsController(IRobotCommandDataAccess repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public ActionResult<IEnumerable<RobotCommand>> GetAllRobotCommands()
            => Ok(_repo.GetRobotCommands());

        [HttpGet("move")]
        public ActionResult<IEnumerable<RobotCommand>> GetMoveCommandsOnly()
            => Ok(_repo.GetRobotCommands().Where(c => c.IsMoveCommand));

        [HttpGet("{id}", Name = "GetRobotCommand")]
        public IActionResult GetRobotCommandById(int id)
        {
            var cmd = _repo.GetRobotCommandById(id);
            return cmd is null
                ? NotFound($"Command with ID {id} not found.")
                : Ok(cmd);
        }

        [HttpPost]
        public IActionResult AddRobotCommand([FromBody] RobotCommand newCmd)
        {
            if (newCmd is null)
                return BadRequest("Request body required.");

            if (_repo.GetRobotCommands()
                     .Any(c => c.Name.Equals(newCmd.Name, StringComparison.OrdinalIgnoreCase)))
            {
                return Conflict($"Command '{newCmd.Name}' already exists.");
            }

            var created = _repo.AddRobotCommand(newCmd);
            return CreatedAtRoute("GetRobotCommand", new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateRobotCommand(int id, [FromBody] RobotCommand upd)
        {
            if (upd is null || upd.Id != id)
                return BadRequest("ID mismatch or body missing.");

            if (_repo.GetRobotCommandById(id) is null)
                return NotFound($"Command with ID {id} not found.");

            upd.ModifiedDate = DateTime.Now;
            return _repo.UpdateRobotCommand(upd)
                ? NoContent()
                : StatusCode(500, "Update failed.");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteRobotCommand(int id)
            => _repo.DeleteRobotCommand(id)
                ? NoContent()
                : NotFound($"Command with ID {id} not found.");
    }
}
