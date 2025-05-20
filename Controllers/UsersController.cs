using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using robot_controller_api.Models;
using robot_controller_api.Persistence;
using robot_controller_api.Services;

namespace robot_controller_api.Controllers
{
    /// <summary>
    /// Handles registration, profile management and credential updates for users.
    /// </summary>
    [ApiController]
    [Route("api/users")]
    [Authorize]  // Require BasicAuth for all endpoints except where overridden
    public class UsersController : ControllerBase
    {
        private readonly IUserDataAccess _userRepo;
        private readonly IPasswordHasherService _passwordHasher;

        public UsersController(
            IUserDataAccess userRepo,
            IPasswordHasherService passwordHasher)    // ← injected
        {
            _userRepo = userRepo;
            _passwordHasher = passwordHasher;          // ← assign
        }

        /// <summary>
        /// GET /api/users
        /// Retrieve all users. Requires authentication.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {
            var users = _userRepo.GetAll();
            return Ok(users);
        }

        /// <summary>
        /// GET /api/users/admin
        /// Retrieve only users with Role = "Admin". Requires authentication.
        /// </summary>
        [HttpGet("admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAdmins()
        {
            var admins = _userRepo.GetByRole("Admin");
            return Ok(admins);
        }

        /// <summary>
        /// GET /api/users/{id}
        /// Retrieve a single user by ID. Requires authentication.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(int id)
        {
            var user = _userRepo.GetById(id);
            if (user is null)
                return NotFound($"User {id} not found.");
            return Ok(user);
        }

        /// <summary>
        /// POST /api/users
        /// Register a new user. Allows anonymous.
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public IActionResult Register(UserModel model)
        {
            if (model == null)
                return BadRequest("UserModel cannot be null.");

            if (_userRepo.EmailExists(model.Email))
                return Conflict("Email already registered.");

            // Set timestamps
            model.CreatedDate = DateTime.UtcNow;
            model.ModifiedDate = model.CreatedDate;

            // Hash the incoming PasswordHash field (which holds plain password)
            model.PasswordHash = _passwordHasher.HashPassword(model, model.PasswordHash);

            var created = _userRepo.Create(model);
            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                created
            );
        }

        /// <summary>
        /// PUT /api/users/{id}
        /// Update profile fields (FirstName, LastName, Description, Role). Requires authentication.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Update(int id, [FromBody] UserUpdateModel m)
        {
            var existing = _userRepo.GetById(id);
            if (existing is null)
                return NotFound($"User {id} not found.");

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            existing.FirstName = m.FirstName;
            existing.LastName = m.LastName;
            existing.Description = m.Description;
            existing.Role = m.Role;
            existing.ModifiedDate = DateTime.UtcNow;

            _userRepo.Update(existing);

            return NoContent();
        }

        /// <summary>
        /// DELETE /api/users/{id}
        /// Delete a user. Requires authentication.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(int id)
        {
            var deleted = _userRepo.Delete(id);
            if (!deleted)
                return NotFound($"User {id} not found.");
            return NoContent();
        }

        /// <summary>
        /// PATCH /api/users/{id}
        /// Change email and/or password. Requires authentication.
        /// </summary>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public IActionResult ChangeCredentials(int id, LoginModel creds)
        {
            if (creds == null)
                return BadRequest("LoginModel cannot be null.");

            var existing = _userRepo.GetById(id);
            if (existing is null)
                return NotFound($"User {id} not found.");

            if (!string.Equals(existing.Email, creds.Email, StringComparison.OrdinalIgnoreCase)
                && _userRepo.EmailExists(creds.Email))
            {
                return Conflict("Email already in use.");
            }

            existing.Email = creds.Email;
            existing.PasswordHash = _passwordHasher.HashPassword(existing, creds.Password);
            existing.ModifiedDate = DateTime.UtcNow;

            _userRepo.Update(existing);
            return NoContent();
        }
    }
}
