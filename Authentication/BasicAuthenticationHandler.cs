using System;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using robot_controller_api.Models;
using robot_controller_api.Persistence;
using robot_controller_api.Services;

namespace robot_controller_api.Authentication
{
    /// <summary>
    /// Handles HTTP Basic Authentication against our Users table.
    /// </summary>
    public class BasicAuthenticationHandler
        : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IUserDataAccess _userRepo;
        private readonly IPasswordHasherService _passwordHasher;

        // Notice: no ISystemClock parameter here, and we call base(...) without it
        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IUserDataAccess userRepo,
            IPasswordHasherService passwordHasher)
            : base(options, logger, encoder)
        {
            _userRepo = userRepo;
            _passwordHasher = passwordHasher;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Advertise Basic realm
            Response.Headers.Append(
                "WWW-Authenticate",
                @"Basic realm=""Access to the robot controller."""
            );

            var header = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(header) || !header.StartsWith("Basic "))
                return Task.FromResult(
                    AuthenticateResult.Fail("Missing or invalid Authorization header")
                );

            string credentials;
            try
            {
                var payload = header.Substring("Basic ".Length).Trim();
                var bytes = Convert.FromBase64String(payload);
                credentials = Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                return Task.FromResult(
                    AuthenticateResult.Fail("Invalid Base64 credentials")
                );
            }

            var parts = credentials.Split(':', 2);
            if (parts.Length != 2)
                return Task.FromResult(
                    AuthenticateResult.Fail("Invalid credential format")
                );

            var email = parts[0];
            var password = parts[1];

            var user = _userRepo.GetByEmail(email);
            if (user == null
                || !_passwordHasher.VerifyHashedPassword(
                       user,
                       user.PasswordHash,
                       password
                   ))
            {
                return Task.FromResult(
                    AuthenticateResult.Fail("Invalid username or password")
                );
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, user.Role ?? "User")
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
