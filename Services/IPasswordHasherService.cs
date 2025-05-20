using robot_controller_api.Models;

namespace robot_controller_api.Services
{
    public interface IPasswordHasherService
    {
        /// <summary>
        /// Hashes the given plain-text password for the specified user.
        /// </summary>
        /// <param name="user">The user record (in case you want to include user-specific data in your salt/rounds).</param>
        /// <param name="plainPassword">The password to hash.</param>
        /// <returns>A salted, hashed password string.</returns>
        string HashPassword(UserModel user, string plainPassword);

        /// <summary>
        /// Verifies that the provided plain-text password matches the stored hash.
        /// </summary>
        /// <param name="user">The user record (if needed by your algorithm).</param>
        /// <param name="hashedPassword">The stored hashed password.</param>
        /// <param name="providedPassword">The plain password to verify.</param>
        /// <returns>True if it matches; false otherwise.</returns>
        bool VerifyHashedPassword(UserModel user, string hashedPassword, string providedPassword);
    }
}
