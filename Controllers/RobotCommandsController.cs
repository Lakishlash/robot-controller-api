using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using robot_controller_api.Models;
using robot_controller_api.Persistence;

namespace robot_controller_api.Controllers
{
    /// <summary>
    /// CRUD and filters for robot commands.
    /// </summary>
    [ApiController]
    [Route("api/robot-commands")]
    [Authorize(Policy = "UserOnly")]
    public class RobotCommandsController : ControllerBase
    {
        private readonly IRobotCommandDataAccess _repo;
        public RobotCommandsController(IRobotCommandDataAccess repo)
            => _repo = repo;

        /// <summary>GET /api/robot-commands → all commands.</summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAll() => Ok(_repo.GetRobotCommands());

        /// <summary>GET /api/robot-commands/move → move commands only.</summary>
        [HttpGet("move")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetMoves() => Ok(_repo.GetMoveCommands());

        /// <summary>GET /api/robot-commands/{id} → single command.</summary>
        [HttpGet("{id}", Name = "GetRobotCommand")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(int id)
        {
            var cmd = _repo.GetRobotCommandById(id);
            return cmd is null
                ? NotFound($"Command {id} not found.")
                : Ok(cmd);
        }

        /// <summary>POST /api/robot-commands → create command.</summary>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Create([FromBody] RobotCommand cmd)
        {
            if (cmd == null) return BadRequest("Body cannot be null.");
            cmd.CreatedDate = DateTime.UtcNow;
            cmd.ModifiedDate = cmd.CreatedDate;
            var created = _repo.AddRobotCommand(cmd);
            return CreatedAtRoute("GetRobotCommand", new { id = created.Id }, created);
        }

        /// <summary>PUT /api/robot-commands/{id} → update command.</summary>
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Update(int id, [FromBody] RobotCommand cmd)
        {
            if (cmd == null || cmd.Id != id)
                return BadRequest("ID mismatch or null body.");

            var existing = _repo.GetRobotCommandById(id);
            if (existing is null)
                return NotFound($"Command {id} not found.");

            cmd.ModifiedDate = DateTime.UtcNow;
            _repo.UpdateRobotCommand(id, cmd);
            return NoContent();
        }

        /// <summary>DELETE /api/robot-commands/{id} → remove command.</summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(int id)
        {
            var deleted = _repo.DeleteRobotCommand(id);
            return deleted
                ? NoContent()
                : NotFound($"Command {id} not found.");
        }
    }
}
