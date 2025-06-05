using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Npgsql;
using robot_controller_api.Models;

namespace robot_controller_api.Persistence
{
    /// <summary>
    /// Repository-pattern implementation of IUserDataAccess for Users.
    /// </summary>
    public class UserRepository : IUserDataAccess
    {
        private readonly string _connString;

        /// <summary>
        /// Read the DefaultConnection from appsettings.json.
        /// </summary>
        public UserRepository(IConfiguration config)
        {
            _connString = config.GetConnectionString("DefaultConnection")!;
        }

        /// <summary>
        /// Map a data-reader row to a UserModel.
        /// </summary>
        private UserModel Map(NpgsqlDataReader dr) => new()
        {
            Id = dr.GetInt32(dr.GetOrdinal("id")),
            Email = dr.GetString(dr.GetOrdinal("email")),
            FirstName = dr.GetString(dr.GetOrdinal("first_name")),
            LastName = dr.GetString(dr.GetOrdinal("last_name")),
            PasswordHash = dr.GetString(dr.GetOrdinal("password_hash")),
            Description = dr.IsDBNull(dr.GetOrdinal("description"))
                           ? null
                           : dr.GetString(dr.GetOrdinal("description")),
            Role = dr.IsDBNull(dr.GetOrdinal("role"))
                           ? null
                           : dr.GetString(dr.GetOrdinal("role")),
            CreatedDate = dr.GetDateTime(dr.GetOrdinal("created_date")),
            ModifiedDate = dr.GetDateTime(dr.GetOrdinal("modified_date"))
        };

        /// <inheritdoc/>
        public IEnumerable<UserModel> GetAll()
        {
            var users = new List<UserModel>();
            using var conn = new NpgsqlConnection(_connString);
            conn.Open();
            using var cmd = new NpgsqlCommand(
                @"SELECT * FROM ""user"" ORDER BY id", conn);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read()) users.Add(Map(rdr));
            return users;
        }

        /// <inheritdoc/>
        public IEnumerable<UserModel> GetByRole(string role)
        {
            var users = new List<UserModel>();
            using var conn = new NpgsqlConnection(_connString);
            conn.Open();
            using var cmd = new NpgsqlCommand(
                @"SELECT * FROM ""user"" WHERE role = @r ORDER BY id", conn);
            cmd.Parameters.AddWithValue("r", role);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read()) users.Add(Map(rdr));
            return users;
        }

        /// <inheritdoc/>
        public UserModel? GetById(int id)
        {
            using var conn = new NpgsqlConnection(_connString);
            conn.Open();
            using var cmd = new NpgsqlCommand(
                @"SELECT * FROM ""user"" WHERE id = @i", conn);
            cmd.Parameters.AddWithValue("i", id);
            using var rdr = cmd.ExecuteReader();
            return rdr.Read() ? Map(rdr) : null;
        }

        /// <inheritdoc/>
        public UserModel? GetByEmail(string email)
        {
            using var conn = new NpgsqlConnection(_connString);
            conn.Open();
            using var cmd = new NpgsqlCommand(
                @"SELECT * FROM ""user"" WHERE email = @e", conn);
            cmd.Parameters.AddWithValue("e", email);
            using var rdr = cmd.ExecuteReader();
            return rdr.Read() ? Map(rdr) : null;
        }

        /// <inheritdoc/>
        public bool EmailExists(string email)
        {
            using var conn = new NpgsqlConnection(_connString);
            conn.Open();
            using var cmd = new NpgsqlCommand(
                @"SELECT COUNT(1) FROM ""user"" WHERE email = @e", conn);
            cmd.Parameters.AddWithValue("e", email);
            return (long)cmd.ExecuteScalar()! > 0;
        }

        /// <inheritdoc/>
        public UserModel Create(UserModel u)
        {
            using var conn = new NpgsqlConnection(_connString);
            conn.Open();
            using var cmd = new NpgsqlCommand(
                @"INSERT INTO ""user""
                  (email, first_name, last_name, password_hash, description, role, created_date, modified_date)
                  VALUES
                  (@e,@f,@l,@p,@d,@r,@cd,@md)
                  RETURNING *;", conn);
            cmd.Parameters.AddWithValue("e", u.Email);
            cmd.Parameters.AddWithValue("f", u.FirstName);
            cmd.Parameters.AddWithValue("l", u.LastName);
            cmd.Parameters.AddWithValue("p", u.PasswordHash);
            cmd.Parameters.AddWithValue("d", (object?)u.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("r", (object?)u.Role ?? DBNull.Value);
            cmd.Parameters.AddWithValue("cd", u.CreatedDate);
            cmd.Parameters.AddWithValue("md", u.ModifiedDate);
            using var rdr = cmd.ExecuteReader();
            rdr.Read();
            return Map(rdr);
        }

        /// <inheritdoc/>
        public void Update(UserModel u)
        {
            using var conn = new NpgsqlConnection(_connString);
            conn.Open();
            using var cmd = new NpgsqlCommand(
                @"UPDATE ""user"" SET
                   email         = @e,
                   first_name    = @f,
                   last_name     = @l,
                   password_hash = @p,
                   description   = @d,
                   role          = @r,
                   modified_date = @md
                 WHERE id = @i;", conn);
            cmd.Parameters.AddWithValue("e", u.Email);
            cmd.Parameters.AddWithValue("f", u.FirstName);
            cmd.Parameters.AddWithValue("l", u.LastName);
            cmd.Parameters.AddWithValue("p", u.PasswordHash);
            cmd.Parameters.AddWithValue("d", (object?)u.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("r", (object?)u.Role ?? DBNull.Value);
            cmd.Parameters.AddWithValue("md", u.ModifiedDate);
            cmd.Parameters.AddWithValue("i", u.Id);
            cmd.ExecuteNonQuery();
        }

        /// <inheritdoc/>
        public bool Delete(int id)
        {
            using var conn = new NpgsqlConnection(_connString);
            conn.Open();
            using var cmd = new NpgsqlCommand(@"DELETE FROM ""user"" WHERE id = @i;", conn);
            cmd.Parameters.AddWithValue("i", id);
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}
