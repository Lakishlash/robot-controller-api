namespace robot_controller_api.Models
{
    /// <summary>
    /// Credentials payload for email/password operations.
    /// </summary>
    public class LoginModel
    {
        /// <summary>New or existing email.</summary>
        public string Email { get; set; } = null!;

        /// <summary>Plain‐text password to hash or verify.</summary>
        public string Password { get; set; } = null!;
    }
}
