using System.Collections.Generic;
using robot_controller_api.Models;

namespace robot_controller_api.Persistence
{
    public interface IMapDataAccess
    {
        List<Map> GetMaps();
        Map? GetMapById(int id);
        Map AddMap(Map newMap);
        bool UpdateMap(Map updatedMap);
        bool DeleteMap(int id);
    }
}
