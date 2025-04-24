using Microsoft.AspNetCore.Mvc;
using RobotControllerApi.Models;
using RobotControllerApi.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RobotControllerApi.Controllers
{
    [ApiController]
    [Route("api/maps")]
    public class MapsController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<Map>> GetAllMaps()
        {
            var all = MapDataAccess.GetMaps();
            return Ok(all);
        }

        [HttpGet("square")]
        public ActionResult<IEnumerable<Map>> GetSquareMaps()
        {
            var square = MapDataAccess
                .GetMaps()
                .Where(m => m.Columns == m.Rows);
            return Ok(square);
        }

        [HttpGet("{id}", Name = "GetMap")]
        public IActionResult GetMapById(int id)
        {
            var map = MapDataAccess.GetMapById(id);
            if (map == null)
                return NotFound($"Map with ID {id} not found.");
            return Ok(map);
        }

        [HttpPost]
        public IActionResult AddMap([FromBody] Map newMap)
        {
            if (newMap == null)
                return BadRequest("Request body required.");

            if (newMap.Columns != newMap.Rows ||
                newMap.Columns < 2 || newMap.Columns > 100)
            {
                return BadRequest("Map must be square and size between 2 and 100.");
            }

            var created = MapDataAccess.AddMap(newMap);
            return CreatedAtRoute(
                "GetMap",
                new { id = created.Id },
                created
            );
        }

        [HttpPut("{id}")]
        public IActionResult UpdateMap(int id, [FromBody] Map upd)
        {
            if (upd == null || upd.Id != id)
                return BadRequest("ID mismatch or body missing.");

            if (upd.Columns != upd.Rows ||
                upd.Columns < 2 || upd.Columns > 100)
            {
                return BadRequest("Map must be square and size between 2 and 100.");
            }

            var existing = MapDataAccess.GetMapById(id);
            if (existing == null)
                return NotFound($"Map with ID {id} not found.");

            upd.ModifiedDate = DateTime.Now;
            bool ok = MapDataAccess.UpdateMap(upd);
            return ok ? NoContent() : StatusCode(500, "Update failed.");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteMap(int id)
        {
            bool deleted = MapDataAccess.DeleteMap(id);
            if (!deleted)
                return NotFound($"Map with ID {id} not found.");
            return NoContent();
        }

        [HttpGet("{id}/{x}-{y}")]
        public IActionResult CheckCoordinate(int id, int x, int y)
        {
            if (x < 0 || y < 0)
                return BadRequest("Coordinates must be non‑negative.");

            var map = MapDataAccess.GetMapById(id);
            if (map == null)
                return NotFound($"Map with ID {id} not found.");

            bool isOnMap = x < map.Columns && y < map.Rows;
            return Ok(isOnMap);
        }
    }
}
