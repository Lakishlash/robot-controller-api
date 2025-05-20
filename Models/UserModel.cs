using System;

namespace robot_controller_api.Models
{
    /// <summary>
    /// Represents an application user for authentication and authorization.
    /// </summary>
    public class UserModel
    {
        /// <summary>Primary key.</summary>
        public int Id { get; set; }

        /// <summary>User’s unique email (login).</summary>
        public string Email { get; set; } = null!;

        /// <summary>First name.</summary>
        public string FirstName { get; set; } = null!;

        /// <summary>Last name.</summary>
        public string LastName { get; set; } = null!;

        /// <summary>Hashed password (via PasswordHasher).</summary>
        public string PasswordHash { get; set; } = null!;

        /// <summary>Optional bio/description.</summary>
        public string? Description { get; set; }

        /// <summary>Role for authorization (“Admin” or “User”).</summary>
        public string? Role { get; set; }

        /// <summary>UTC created timestamp.</summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>UTC last‐modified timestamp.</summary>
        public DateTime ModifiedDate { get; set; }
    }
}
