using System.Collections.Generic;
using robot_controller_api.Models;          

namespace robot_controller_api.Persistence
{
    public interface IRobotCommandDataAccess
    {
        List<RobotCommand> GetRobotCommands();
        RobotCommand? GetRobotCommandById(int id);
        RobotCommand AddRobotCommand(RobotCommand newCmd);
        bool UpdateRobotCommand(RobotCommand cmdToUpdate);
        bool DeleteRobotCommand(int id);
    }
}
