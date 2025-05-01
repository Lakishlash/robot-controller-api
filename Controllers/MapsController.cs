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

        [HttpGet]
        public ActionResult<IEnumerable<Map>> GetAllMaps()
            => Ok(_repo.GetMaps());

        [HttpGet("square")]
        public ActionResult<IEnumerable<Map>> GetSquareMaps()
            => Ok(_repo.GetMaps().Where(m => m.Columns == m.Rows));

        [HttpGet("{id}", Name = "GetMap")]
        public IActionResult GetMapById(int id)
        {
            var map = _repo.GetMapById(id);
            return map is null
                ? NotFound($"Map with ID {id} not found.")
                : Ok(map);
        }

        [HttpPost]
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

        [HttpPut("{id}")]
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

        [HttpDelete("{id}")]
        public IActionResult DeleteMap(int id)
            => _repo.DeleteMap(id)
                ? NoContent()
                : NotFound($"Map with ID {id} not found.");

        [HttpGet("{id}/{x}-{y}")]
        public IActionResult CheckCoordinate(int id, int x, int y)
        {
            if (x < 0 || y < 0)
                return BadRequest("Coordinates must be non‐negative.");

            var map = _repo.GetMapById(id);
            if (map is null)
                return NotFound($"Map with ID {id} not found.");

            return Ok(x < map.Columns && y < map.Rows);
        }
    }
}
