using Microsoft.AspNetCore.Mvc;
using RobotControllerApi.Models;
using RobotControllerApi.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RobotControllerApi.Controllers
{
    [ApiController]
    [Route("api/robot-commands")]
    public class RobotCommandsController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<RobotCommand>> GetAllRobotCommands()
        {
            var all = RobotCommandDataAccess.GetRobotCommands();
            return Ok(all);
        }

        [HttpGet("move")]
        public ActionResult<IEnumerable<RobotCommand>> GetMoveCommandsOnly()
        {
            var moves = RobotCommandDataAccess
                .GetRobotCommands()
                .Where(c => c.IsMoveCommand);
            return Ok(moves);
        }

        [HttpGet("{id}", Name = "GetRobotCommand")]
        public IActionResult GetRobotCommandById(int id)
        {
            var cmd = RobotCommandDataAccess.GetRobotCommandById(id);
            if (cmd == null)
                return NotFound($"Command with ID {id} not found.");
            return Ok(cmd);
        }

        [HttpPost]
        public IActionResult AddRobotCommand([FromBody] RobotCommand newCmd)
        {
            if (newCmd == null)
                return BadRequest("Request body required.");

            // Prevent duplicate names
            var exists = RobotCommandDataAccess
                .GetRobotCommands()
                .Any(c => c.Name.Equals(newCmd.Name, StringComparison.OrdinalIgnoreCase));
            if (exists)
                return Conflict($"Command '{newCmd.Name}' already exists.");

            var created = RobotCommandDataAccess.AddRobotCommand(newCmd);
            return CreatedAtRoute(
                "GetRobotCommand",
                new { id = created.Id },
                created
            );
        }

        [HttpPut("{id}")]
        public IActionResult UpdateRobotCommand(int id, [FromBody] RobotCommand upd)
        {
            if (upd == null || upd.Id != id)
                return BadRequest("ID mismatch or body missing.");

            var existing = RobotCommandDataAccess.GetRobotCommandById(id);
            if (existing == null)
                return NotFound($"Command with ID {id} not found.");

            upd.ModifiedDate = DateTime.Now;
            bool ok = RobotCommandDataAccess.UpdateRobotCommand(upd);
            return ok ? NoContent() : StatusCode(500, "Update failed.");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteRobotCommand(int id)
        {
            bool deleted = RobotCommandDataAccess.DeleteRobotCommand(id);
            if (!deleted)
                return NotFound($"Command with ID {id} not found.");
            return NoContent();
        }
    }
}
