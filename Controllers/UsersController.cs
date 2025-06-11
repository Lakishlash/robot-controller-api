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
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserDataAccess _userRepo;
        private readonly IPasswordHasherService _passwordHasher;

        public UsersController(
            IUserDataAccess userRepo,
            IPasswordHasherService passwordHasher)
        {
            _userRepo = userRepo;
            _passwordHasher = passwordHasher;
        }

        // GET: api/users (admin only)
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult GetAll()
        {
            var users = _userRepo.GetAll();
            return Ok(users);
        }

        // GET: api/users/by-role/{role}
        [HttpGet("by-role/{role}")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult GetByRole(string role)
        {
            var users = _userRepo.GetByRole(role);
            return Ok(users);
        }

        // GET: api/users/{id} (self or admin)
        [HttpGet("{id:int}")]
        [Authorize(Policy = "UserOnly")]
        public IActionResult GetById(int id)
        {
            var user = _userRepo.GetById(id);
            if (user == null)
                return NotFound($"User with ID {id} not found.");
            return Ok(user);
        }

        // GET: api/users/by-email/{email} (self or admin)
        [HttpGet("by-email/{email}")]
        [Authorize(Policy = "UserOnly")]
        public IActionResult GetByEmail(string email)
        {
            var user = _userRepo.GetByEmail(email);
            if (user == null)
                return NotFound($"User {email} not found.");
            return Ok(user);
        }

        // POST: api/users (registration)
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Register([FromBody] UserModel newUser)
        {
            if (newUser == null) return BadRequest("Body cannot be null.");
            if (_userRepo.EmailExists(newUser.Email))
                return Conflict("A user with this email already exists.");

            // Hash password before saving!
            newUser.PasswordHash = _passwordHasher.HashPassword(newUser, newUser.PasswordHash);
            newUser.CreatedDate = DateTime.UtcNow;
            newUser.ModifiedDate = newUser.CreatedDate;

            var created = _userRepo.Create(newUser);
            return CreatedAtAction(nameof(GetByEmail), new { email = created.Email }, created);
        }

        // PUT: api/users/{id} (self or admin)
        [HttpPut("{id:int}")]
        [Authorize(Policy = "UserOnly")]
        public IActionResult Update(int id, [FromBody] UserModel updatedUser)
        {
            if (updatedUser == null || updatedUser.Id != id)
                return BadRequest("ID mismatch or null body.");

            var existing = _userRepo.GetById(id);
            if (existing == null)
                return NotFound($"User with ID {id} not found.");

            // Optional: re-hash password if changed
            if (!string.IsNullOrWhiteSpace(updatedUser.PasswordHash) &&
                updatedUser.PasswordHash != existing.PasswordHash)
            {
                updatedUser.PasswordHash = _passwordHasher.HashPassword(updatedUser, updatedUser.PasswordHash);
            }

            updatedUser.ModifiedDate = DateTime.UtcNow;
            _userRepo.Update(updatedUser);
            return NoContent();
        }

        // PATCH: api/users/{id}/credentials (self or admin)
        [HttpPatch("{id:int}/credentials")]
        [Authorize(Policy = "UserOnly")]
        public IActionResult PatchCredentials(int id, [FromBody] UserCredentialUpdateModel credentials)
        {
            if (credentials == null)
                return BadRequest("Credentials object is null.");

            var existing = _userRepo.GetById(id);
            if (existing == null)
                return NotFound($"User with ID {id} not found.");

            bool changed = false;

            // Only update email if provided
            if (!string.IsNullOrWhiteSpace(credentials.Email))
            {
                existing.Email = credentials.Email;
                changed = true;
            }

            // Only update password if provided
            if (!string.IsNullOrWhiteSpace(credentials.Password))
            {
                existing.PasswordHash = _passwordHasher.HashPassword(existing, credentials.Password);
                changed = true;
            }

            if (changed)
            {
                existing.ModifiedDate = DateTime.UtcNow;
                _userRepo.Update(existing);
            }

            return NoContent();
        }


        // DELETE: api/users/{id} (admin only)
        [HttpDelete("{id:int}")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Delete(int id)
        {
            var existing = _userRepo.GetById(id);
            if (existing == null)
                return NotFound($"User with ID {id} not found.");

            var deleted = _userRepo.Delete(id);
            return deleted ? NoContent() : StatusCode(StatusCodes.Status500InternalServerError, "Delete failed.");
        }

        /// <summary>
        /// Only accessible to users whose first name starts with A (policy enforced)
        /// </summary>
        [HttpGet("starts-with-a")]
        [Authorize(Policy = "NameStartsWithAOnly")]
        public IActionResult ForUsersStartingWithA()
        {
            return Ok("Congrats! Your first name starts with 'A'. You passed the custom claim policy!");
        }
    }
}
