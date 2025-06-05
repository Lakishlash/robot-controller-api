using System.ComponentModel.DataAnnotations;

namespace robot_controller_api.Models
{
    /// <summary>
    /// Payload for updating a user’s profile fields only.
    /// </summary>
    public class UserUpdateModel
    {
        /// <summary>First name.</summary>
        [Required]
        public string FirstName { get; set; } = null!;

        /// <summary>Last name.</summary>
        [Required]
        public string LastName { get; set; } = null!;

        /// <summary>Optional bio/description.</summary>
        public string? Description { get; set; }

        /// <summary>Role for authorization (“Admin” or “User”).</summary>
        public string? Role { get; set; }
    }
}
