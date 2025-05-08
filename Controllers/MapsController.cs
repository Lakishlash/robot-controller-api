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
    [Route("api/maps")]
    public class MapsController : ControllerBase
    {
        private readonly IMapDataAccess _repo;

        public MapsController(IMapDataAccess repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Retrieves all saved maps.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Map>> GetAllMaps()
            => Ok(_repo.GetMaps());

        /// <summary>
        /// Retrieves only the square maps (columns == rows).
        /// </summary>
        [HttpGet("square")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Map>> GetSquareMaps()
            => Ok(_repo.GetMaps().Where(m => m.Columns == m.Rows));

        /// <summary>
        /// Retrieves a single map by its ID.
        /// </summary>
        /// <param name="id">ID of the map to fetch.</param>
        [HttpGet("{id}", Name = "GetMap")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetMapById(int id)
        {
            var map = _repo.GetMapById(id);
            return map is null
                ? NotFound($"Map with ID {id} not found.")
                : Ok(map);
        }

        /// <summary>
        /// Creates a new square map.
        /// </summary>
        /// <param name="newMap">Map payload (must be square, size 2–100).</param>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/maps
        ///     {
        ///       "columns": 5,
        ///       "rows": 5,
        ///       "name": "Test5x5",
        ///       "description": "A 5x5 grid"
        ///     }
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AddMap([FromBody] Map newMap)
        {
            if (newMap is null)
                return BadRequest("Request body required.");

            if (newMap.Columns != newMap.Rows ||
                newMap.Columns < 2 || newMap.Columns > 100)
            {
                return BadRequest("Map must be square and size between 2 and 100.");
            }

            var created = _repo.AddMap(newMap);
            return CreatedAtRoute("GetMap", new { id = created.Id }, created);
        }

        /// <summary>
        /// Updates an existing map.
        /// </summary>
        /// <param name="id">Map ID to update.</param>
        /// <param name="upd">Updated map payload.</param>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateMap(int id, [FromBody] Map upd)
        {
            if (upd is null || upd.Id != id)
                return BadRequest("ID mismatch or body missing.");

            if (upd.Columns != upd.Rows ||
                upd.Columns < 2 || upd.Columns > 100)
            {
                return BadRequest("Map must be square and size between 2 and 100.");
            }

            if (_repo.GetMapById(id) is null)
                return NotFound($"Map with ID {id} not found.");

            upd.ModifiedDate = DateTime.Now;
            return _repo.UpdateMap(upd)
                ? NoContent()
                : StatusCode(500, "Update failed.");
        }

        /// <summary>
        /// Deletes a map by ID.
        /// </summary>
        /// <param name="id">ID of the map to delete.</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteMap(int id)
            => _repo.DeleteMap(id)
                ? NoContent()
                : NotFound($"Map with ID {id} not found.");

        /// <summary>
        /// Checks whether a given (x,y) coordinate lies within the map bounds.
        /// </summary>
        /// <param name="id">Map ID.</param>
        /// <param name="x">Column index (0-based).</param>
        /// <param name="y">Row index (0-based).</param>
        [HttpGet("{id}/{x}-{y}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult CheckCoordinate(int id, int x, int y)
        {
            if (x < 0 || y < 0)
                return BadRequest("Coordinates must be non-negative.");

            var map = _repo.GetMapById(id);
            if (map is null)
                return NotFound($"Map with ID {id} not found.");

            return Ok(x < map.Columns && y < map.Rows);
        }
    }
}
