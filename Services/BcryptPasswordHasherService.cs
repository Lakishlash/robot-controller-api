using robot_controller_api.Models;
using BCrypt.Net;

namespace robot_controller_api.Services
{
    public class BcryptPasswordHasherService : IPasswordHasherService
    {
        private const int WorkFactor = 12; // adjust via config later, not hard-coded in methods

        public string HashPassword(UserModel user, string plainPassword)
        {
            // This generates a random salt internally and applies the work factor
            return BCrypt.Net.BCrypt.HashPassword(plainPassword, WorkFactor);
        }

        public bool VerifyHashedPassword(UserModel user, string hashedPassword, string providedPassword)
        {
            // Compares the provided password against the stored hash
            return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
        }
    }
}
