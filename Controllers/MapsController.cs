using Microsoft.AspNetCore.Mvc;
using RobotControllerApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RobotControllerApi.Controllers
{
    [ApiController]
    [Route("api/maps")]
    public class MapsController : ControllerBase
    {
        // In-memory list to hold maps
        private static readonly List<Map> _maps = new List<Map>
        {
            new Map(1, 5, 5, "MOON", null, DateTime.Now, DateTime.Now)
        };

        [HttpGet]
        public IEnumerable<Map> GetAllMaps() => _maps;

        [HttpGet("square")]
        public IEnumerable<Map> GetSquareMaps() =>
            _maps.Where(m => m.Columns == m.Rows);

        [HttpGet("{id}")]
        public IActionResult GetMapById(int id)
        {
            var map = _maps.FirstOrDefault(m => m.Id == id);
            if (map == null)
                return NotFound($"Map with ID {id} not found.");
            return Ok(map);
        }

        [HttpPost]
        public IActionResult AddMap([FromBody] Map newMap)
        {
            if (newMap == null)
                return BadRequest();
            if (newMap.Columns != newMap.Rows || newMap.Columns < 2 || newMap.Columns > 100)
                return BadRequest("Map must be square and size must be between 2 and 100.");

            int newId = _maps.Max(m => m.Id) + 1;
            newMap.Id = newId;
            newMap.CreatedDate = DateTime.Now;
            newMap.ModifiedDate = DateTime.Now;
            _maps.Add(newMap);
            return CreatedAtAction(nameof(GetMapById), new { id = newMap.Id }, newMap);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateMap(int id, [FromBody] Map updatedMap)
        {
            var existing = _maps.FirstOrDefault(m => m.Id == id);
            if (existing == null)
                return NotFound($"Map with ID {id} not found.");

            if (updatedMap.Columns != updatedMap.Rows || updatedMap.Columns < 2 || updatedMap.Columns > 100)
                return BadRequest("Map must be square and size must be between 2 and 100.");

            existing.Name = updatedMap.Name;
            existing.Description = updatedMap.Description;
            existing.Columns = updatedMap.Columns;
            existing.Rows = updatedMap.Rows;
            existing.ModifiedDate = DateTime.Now;
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteMap(int id)
        {
            var map = _maps.FirstOrDefault(m => m.Id == id);
            if (map == null)
                return NotFound($"Map with ID {id} not found.");
            _maps.Remove(map);
            return NoContent();
        }

        [HttpGet("{id}/{x}-{y}")]
        public IActionResult CheckCoordinate(int id, int x, int y)
        {
            if (x < 0 || y < 0)
                return BadRequest("Coordinates must be non-negative.");
            var map = _maps.FirstOrDefault(m => m.Id == id);
            if (map == null)
                return NotFound($"Map with ID {id} not found.");

            bool isOnMap = x < map.Columns && y < map.Rows;
            return Ok(isOnMap);
        }
    }
}
