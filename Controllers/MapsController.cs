using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using robot_controller_api.Models;
using robot_controller_api.Persistence;

namespace robot_controller_api.Controllers
{
    /// <summary>
    /// CRUD, filters and coordinate checks for maps.
    /// </summary>
    [ApiController]
    [Route("api/maps")]
    [Authorize(Policy = "UserOnly")]
    public class MapsController : ControllerBase
    {
        private readonly IMapDataAccess _repo;
        public MapsController(IMapDataAccess repo) => _repo = repo;

        /// <summary>GET /api/maps → all maps.</summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAll() => Ok(_repo.GetMaps());

        /// <summary>GET /api/maps/square → only square maps.</summary>
        [HttpGet("square")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetSquares() => Ok(_repo.GetSquareMaps());

        /// <summary>GET /api/maps/{id} → single map.</summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(int id)
        {
            var map = _repo.GetMapById(id);
            return map is null
                ? NotFound($"Map {id} not found.")
                : Ok(map);
        }

        /// <summary>GET /api/maps/{id}/{x}-{y} → coordinate check.</summary>
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
                return NotFound($"Map {id} not found.");
            return Ok(_repo.CheckCoordinate(id, x, y));
        }

        /// <summary>POST /api/maps → create map.</summary>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Create([FromBody] Map m)
        {
            if (m == null) return BadRequest("Body cannot be null.");
            m.CreatedDate = DateTime.UtcNow;
            m.ModifiedDate = m.CreatedDate;
            var created = _repo.AddMap(m);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>PUT /api/maps/{id} → update map.</summary>
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Update(int id, [FromBody] Map m)
        {
            if (m == null || m.Id != id)
                return BadRequest("ID mismatch or null body.");

            var existing = _repo.GetMapById(id);
            if (existing is null)
                return NotFound($"Map {id} not found.");

            m.ModifiedDate = DateTime.UtcNow;
            _repo.UpdateMap(id, m);
            return NoContent();
        }

        /// <summary>DELETE /api/maps/{id} → remove map.</summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(int id)
        {
            var deleted = _repo.DeleteMap(id);
            return deleted
                ? NoContent()
                : NotFound($"Map {id} not found.");
        }
    }
}
