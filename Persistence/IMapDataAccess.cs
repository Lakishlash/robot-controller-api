using System.Collections.Generic;
using robot_controller_api.Models;

namespace robot_controller_api.Persistence
{
    /// <summary>
    /// Data access abstraction for Map entities, matching the PostgreSQL schema.
    /// </summary>
    public interface IMapDataAccess
    {
        /// <summary>
        /// Retrieve all maps.
        /// </summary>
        IEnumerable<Map> GetMaps();

        /// <summary>
        /// Retrieve only maps that are square (rows == columns).
        /// </summary>
        IEnumerable<Map> GetSquareMaps();

        /// <summary>
        /// Retrieve a single map by its ID.
        /// </summary>
        /// <param name="id">The map’s ID.</param>
        /// <returns>The <see cref="Map"/>, or <c>null</c> if not found.</returns>
        Map? GetMapById(int id);

        /// <summary>
        /// Add a new map to the database.
        /// </summary>
        /// <param name="mapModel">The map to add.</param>
        /// <returns>The newly created <see cref="Map"/>, including its assigned ID.</returns>
        Map AddMap(Map mapModel);

        /// <summary>
        /// Update an existing map.
        /// </summary>
        /// <param name="id">ID of the map to update.</param>
        /// <param name="mapModel">New map data.</param>
        void UpdateMap(int id, Map mapModel);

        /// <summary>
        /// Delete a map by ID.
        /// </summary>
        /// <param name="id">ID of the map to remove.</param>
        /// <returns><c>true</c> if a row was deleted; otherwise <c>false</c>.</returns>
        bool DeleteMap(int id);

        /// <summary>
        /// Check whether the given (x,y) coordinate lies inside the bounds of the map.
        /// </summary>
        /// <param name="id">Map ID.</param>
        /// <param name="x">Column coordinate (0–Columns-1).</param>
        /// <param name="y">Row coordinate (0–Rows-1).</param>
        /// <returns><c>true</c> if the coordinate is on the map; otherwise <c>false</c>.</returns>
        bool CheckCoordinate(int id, int x, int y);
    }
}
