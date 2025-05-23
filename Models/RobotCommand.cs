using System;

namespace robot_controller_api.Models
{
    public class RobotCommand
    {
        public int Id { get; set; }

        
        public string Name { get; set; } = default!;

        public string? Description { get; set; }
        public bool IsMoveCommand { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        // parameterless ctor for ORM
        public RobotCommand() { }

        public RobotCommand(
            int id,
            string name,
            bool isMoveCommand,
            DateTime createdDate,
            DateTime modifiedDate,
            string? description = null)
        {
            Id = id;
            Name = name;
            IsMoveCommand = isMoveCommand;
            CreatedDate = createdDate;
            ModifiedDate = modifiedDate;
            Description = description;
        }
    }
}
