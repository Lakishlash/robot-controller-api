using System;
using System.Text;
using Konscious.Security.Cryptography;
using robot_controller_api.Models;
using System.Security.Cryptography;

namespace robot_controller_api.Services
{

    public class Argon2PasswordHasherService : IPasswordHasherService
    {
        private const int SaltSize = 16; // 128 bits
        private const int HashSize = 32; // 256 bits
        private const int Iterations = 4;
        private const int MemorySize = 65536; // 64 MB

        public string HashPassword(UserModel user, string password)
        {
            // Generate a random salt
            byte[] salt = new byte[SaltSize];
            RandomNumberGenerator.Fill(salt);

            // Hash password with Argon2id
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = 2,
                Iterations = Iterations,
                MemorySize = MemorySize
            };
            byte[] hash = argon2.GetBytes(HashSize);

            // Store salt and hash together as base64(salt):base64(hash)
            return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
        }

        public bool VerifyHashedPassword(UserModel user, string hashedPassword, string providedPassword)
        {
            // Split the stored salt and hash
            var parts = hashedPassword.Split(':');
            if (parts.Length != 2) return false;

            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] expectedHash = Convert.FromBase64String(parts[1]);

            // Hash the provided password with the same salt and parameters
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(providedPassword))
            {
                Salt = salt,
                DegreeOfParallelism = 2,
                Iterations = Iterations,
                MemorySize = MemorySize
            };
            byte[] actualHash = argon2.GetBytes(HashSize);

            // Use fixed-time comparison for security
            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }
    }
}
