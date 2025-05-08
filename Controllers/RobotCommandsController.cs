using Microsoft.AspNetCore.Http;
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

        /// <summary>
        /// Retrieves all robot commands.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<RobotCommand>> GetAllRobotCommands()
            => Ok(_repo.GetRobotCommands());

        /// <summary>
        /// Retrieves only the move commands (IsMoveCommand = true).
        /// </summary>
        [HttpGet("move")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<RobotCommand>> GetMoveCommandsOnly()
            => Ok(_repo.GetRobotCommands().Where(c => c.IsMoveCommand));

        /// <summary>
        /// Retrieves a single command by its ID.
        /// </summary>
        /// <param name="id">ID of the robot command to fetch.</param>
        [HttpGet("{id}", Name = "GetRobotCommand")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetRobotCommandById(int id)
        {
            var cmd = _repo.GetRobotCommandById(id);
            return cmd is null
                ? NotFound($"Command with ID {id} not found.")
                : Ok(cmd);
        }

        /// <summary>
        /// Creates a new robot command.
        /// </summary>
        /// <param name="newCmd">The command object to create.</param>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/robot-commands
        ///     {
        ///       "name": "MoveForward",
        ///       "isMoveCommand": true,
        ///       "description": "Step ahead"
        ///     }
        ///
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
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
            return CreatedAtRoute("GetRobotCommand",
                                  new { id = created.Id },
                                  created);
        }

        /// <summary>
        /// Updates an existing robot command.
        /// </summary>
        /// <param name="id">ID of the command to update.</param>
        /// <param name="upd">Updated command object (must match ID in URL).</param>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Deletes a robot command by ID.
        /// </summary>
        /// <param name="id">ID of the command to delete.</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteRobotCommand(int id)
            => _repo.DeleteRobotCommand(id)
                ? NoContent()
                : NotFound($"Command with ID {id} not found.");
    }
}
