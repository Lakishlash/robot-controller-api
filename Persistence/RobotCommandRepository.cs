using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Npgsql;
using robot_controller_api.Models;

namespace robot_controller_api.Persistence
{
    /// <summary>
    /// Repository pattern for RobotCommand, using ADO.NET.
    /// </summary>
    public class RobotCommandRepository : IRobotCommandDataAccess
    {
        private readonly string _cs;
        public RobotCommandRepository(IConfiguration cfg)
            => _cs = cfg.GetConnectionString("DefaultConnection")!;

        /// <summary>robotcommand row → RobotCommand.</summary>
        private RobotCommand Map(NpgsqlDataReader dr)
            => new()
            {
                Id = dr.GetInt32(dr.GetOrdinal("id")),
                Name = dr.GetString(dr.GetOrdinal("name")),
                Description = dr.IsDBNull(dr.GetOrdinal("description"))
                                  ? null
                                  : dr.GetString(dr.GetOrdinal("description")),
                IsMoveCommand = dr.GetBoolean(dr.GetOrdinal("ismovecommand")),
                CreatedDate = dr.GetDateTime(dr.GetOrdinal("createddate")),
                ModifiedDate = dr.GetDateTime(dr.GetOrdinal("modifieddate"))
            };

        public IEnumerable<RobotCommand> GetRobotCommands()
        {
            var list = new List<RobotCommand>();
            using var conn = new NpgsqlConnection(_cs);
            conn.Open();
            using var cmd = new NpgsqlCommand(@"
                SELECT id, name, description, ismovecommand, createddate, modifieddate
                  FROM public.robotcommand
                 ORDER BY id", conn);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
                list.Add(Map(rdr));
            return list;
        }

        public IEnumerable<RobotCommand> GetMoveCommands()
        {
            var list = new List<RobotCommand>();
            using var conn = new NpgsqlConnection(_cs);
            conn.Open();
            using var cmd = new NpgsqlCommand(@"
                SELECT id, name, description, ismovecommand, createddate, modifieddate
                  FROM public.robotcommand
                 WHERE ismovecommand = TRUE
                 ORDER BY id", conn);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
                list.Add(Map(rdr));
            return list;
        }

        public RobotCommand? GetRobotCommandById(int id)
        {
            using var conn = new NpgsqlConnection(_cs);
            conn.Open();
            using var cmd = new NpgsqlCommand(@"
                SELECT id, name, description, ismovecommand, createddate, modifieddate
                  FROM public.robotcommand
                 WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("id", id);
            using var rdr = cmd.ExecuteReader();
            return rdr.Read() ? Map(rdr) : null;
        }

        public RobotCommand AddRobotCommand(RobotCommand model)
        {
            using var conn = new NpgsqlConnection(_cs);
            conn.Open();
            using var cmd = new NpgsqlCommand(@"
                INSERT INTO public.robotcommand
                  (name, description, ismovecommand, createddate, modifieddate)
                VALUES
                  (@name, @description, @ismovecommand, @createddate, @modifieddate)
                RETURNING id, name, description, ismovecommand, createddate, modifieddate;", conn);

            cmd.Parameters.AddWithValue("name", model.Name);
            if (model.Description is null)
                cmd.Parameters.AddWithValue("description", DBNull.Value);
            else
                cmd.Parameters.AddWithValue("description", model.Description);

            cmd.Parameters.AddWithValue("ismovecommand", model.IsMoveCommand);
            cmd.Parameters.AddWithValue("createddate", model.CreatedDate);
            cmd.Parameters.AddWithValue("modifieddate", model.ModifiedDate);

            using var rdr = cmd.ExecuteReader();
            rdr.Read();
            return Map(rdr);
        }

        public void UpdateRobotCommand(int id, RobotCommand model)
        {
            using var conn = new NpgsqlConnection(_cs);
            conn.Open();
            using var cmd = new NpgsqlCommand(@"
                UPDATE public.robotcommand
                   SET name          = @name,
                       description   = @description,
                       ismovecommand = @ismovecommand,
                       modifieddate  = @modifieddate
                 WHERE id = @id", conn);

            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("name", model.Name);
            if (model.Description is null)
                cmd.Parameters.AddWithValue("description", DBNull.Value);
            else
                cmd.Parameters.AddWithValue("description", model.Description);

            cmd.Parameters.AddWithValue("ismovecommand", model.IsMoveCommand);
            cmd.Parameters.AddWithValue("modifieddate", model.ModifiedDate);

            cmd.ExecuteNonQuery();
        }

        public bool DeleteRobotCommand(int id)
        {
            using var conn = new NpgsqlConnection(_cs);
            conn.Open();
            using var cmd = new NpgsqlCommand(@"
                DELETE FROM public.robotcommand
                 WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("id", id);
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}
