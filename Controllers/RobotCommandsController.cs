using Microsoft.AspNetCore.Mvc;
using RobotControllerApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RobotControllerApi.Controllers
{
    [ApiController]
    [Route("api/robot-commands")]
    public class RobotCommandsController : ControllerBase
    {
        // In-memory list to hold commands
        private static readonly List<RobotCommand> _commands = new List<RobotCommand>
        {
            new RobotCommand(1, "PLACE", false, DateTime.Now, DateTime.Now),
            new RobotCommand(2, "MOVE", true, DateTime.Now, DateTime.Now),
            new RobotCommand(3, "LEFT", true, DateTime.Now, DateTime.Now),
            new RobotCommand(4, "RIGHT", true, DateTime.Now, DateTime.Now),
            new RobotCommand(5, "REPORT", false, DateTime.Now, DateTime.Now)
        };

        [HttpGet]
        public IEnumerable<RobotCommand> GetAllRobotCommands() => _commands;

        [HttpGet("move")]
        public IEnumerable<RobotCommand> GetMoveCommandsOnly() =>
            _commands.Where(c => c.IsMoveCommand);

        [HttpGet("{id}", Name = "GetRobotCommand")]
        public IActionResult GetRobotCommandById(int id)
        {
            var command = _commands.FirstOrDefault(c => c.Id == id);
            if (command == null)
                return NotFound($"Command with ID {id} not found.");
            return Ok(command);
        }

        [HttpPost]
        public IActionResult AddRobotCommand([FromBody] RobotCommand newCommand)
        {
            if (newCommand == null)
                return BadRequest();
            if (_commands.Any(c => c.Name.Equals(newCommand.Name, StringComparison.OrdinalIgnoreCase)))
                return Conflict($"Command with name {newCommand.Name} already exists.");

            int newId = _commands.Max(c => c.Id) + 1;
            newCommand.Id = newId;
            newCommand.CreatedDate = DateTime.Now;
            newCommand.ModifiedDate = DateTime.Now;
            _commands.Add(newCommand);
            return CreatedAtRoute("GetRobotCommand", new { id = newCommand.Id }, newCommand);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateRobotCommand(int id, [FromBody] RobotCommand updatedCommand)
        {
            var existing = _commands.FirstOrDefault(c => c.Id == id);
            if (existing == null)
                return NotFound($"Command with ID {id} not found.");

            existing.Name = updatedCommand.Name;
            existing.IsMoveCommand = updatedCommand.IsMoveCommand;
            existing.Description = updatedCommand.Description;
            existing.ModifiedDate = DateTime.Now;
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteRobotCommand(int id)
        {
            var command = _commands.FirstOrDefault(c => c.Id == id);
            if (command == null)
                return NotFound($"Command with ID {id} not found.");
            _commands.Remove(command);
            return NoContent();
        }
    }
}
