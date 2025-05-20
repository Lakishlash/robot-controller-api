using System.Collections.Generic;
using robot_controller_api.Models;

namespace robot_controller_api.Persistence
{
    /// <summary>
    /// CRUD and filters for RobotCommand.
    /// </summary>
    public interface IRobotCommandDataAccess
    {
        IEnumerable<RobotCommand> GetRobotCommands();
        IEnumerable<RobotCommand> GetMoveCommands();
        RobotCommand? GetRobotCommandById(int id);
        RobotCommand AddRobotCommand(RobotCommand cmd);
        void UpdateRobotCommand(int id, RobotCommand cmd);
        bool DeleteRobotCommand(int id);
    }
}
