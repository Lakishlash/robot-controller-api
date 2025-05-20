// Persistence/MapRepository.cs

using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Npgsql;
using robot_controller_api.Models;

namespace robot_controller_api.Persistence
{
    /// <summary>
    /// ADO.NET-style repository for Map, matching your Postgres schema.
    /// </summary>
    public class MapRepository : IMapDataAccess
    {
        private readonly string _cs;
        public MapRepository(IConfiguration cfg)
            => _cs = cfg.GetConnectionString("DefaultConnection")!;

        /// <summary>Helper: map one data-row → Map.</summary>
        private Map Map(NpgsqlDataReader dr) => new Map
        {
            Id = dr.GetInt32(dr.GetOrdinal("id")),
            Name = dr.GetString(dr.GetOrdinal("name")),
            Rows = dr.GetInt32(dr.GetOrdinal("rows")),
            Columns = dr.GetInt32(dr.GetOrdinal("columns")),
            Description = dr.IsDBNull(dr.GetOrdinal("description"))
                           ? null
                           : dr.GetString(dr.GetOrdinal("description")),
            IsSquare = dr.GetBoolean(dr.GetOrdinal("issquare")),
            CreatedDate = dr.GetDateTime(dr.GetOrdinal("createddate")),
            ModifiedDate = dr.GetDateTime(dr.GetOrdinal("modifieddate")),
        };

        public IEnumerable<Map> GetMaps()
        {
            var list = new List<Map>();
            using var conn = new NpgsqlConnection(_cs);
            conn.Open();
            using var cmd = new NpgsqlCommand(
                @"SELECT id,name,rows,columns,description,issquare,createddate,modifieddate
                  FROM map
                  ORDER BY id", conn);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read()) list.Add(Map(rdr));
            return list;
        }

        public IEnumerable<Map> GetSquareMaps()
        {
            var list = new List<Map>();
            using var conn = new NpgsqlConnection(_cs);
            conn.Open();
            using var cmd = new NpgsqlCommand(
                @"SELECT id,name,rows,columns,description,issquare,createddate,modifieddate
                  FROM map
                  WHERE issquare = TRUE
                  ORDER BY id", conn);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read()) list.Add(Map(rdr));
            return list;
        }

        public Map? GetMapById(int id)
        {
            using var conn = new NpgsqlConnection(_cs);
            conn.Open();
            using var cmd = new NpgsqlCommand(
                @"SELECT id,name,rows,columns,description,issquare,createddate,modifieddate
                  FROM map
                  WHERE id = @i", conn);
            cmd.Parameters.AddWithValue("i", id);
            using var rdr = cmd.ExecuteReader();
            return rdr.Read()
                ? Map(rdr)
                : null;
        }

        public Map AddMap(Map mapModel)
        {
            using var conn = new NpgsqlConnection(_cs);
            conn.Open();
            using var cmd = new NpgsqlCommand(
                @"INSERT INTO map
                  (name,rows,columns,description,createddate,modifieddate)
                  VALUES
                  (@n,@r,@c,@d,@cd,@md)
                  RETURNING id,name,rows,columns,description,issquare,createddate,modifieddate", conn);
            cmd.Parameters.AddWithValue("n", mapModel.Name);
            cmd.Parameters.AddWithValue("r", mapModel.Rows);
            cmd.Parameters.AddWithValue("c", mapModel.Columns);
            cmd.Parameters.AddWithValue("d", (object?)mapModel.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("cd", mapModel.CreatedDate);
            cmd.Parameters.AddWithValue("md", mapModel.ModifiedDate);
            using var rdr = cmd.ExecuteReader();
            rdr.Read();
            return Map(rdr);
        }

        public void UpdateMap(int id, Map mapModel)
        {
            using var conn = new NpgsqlConnection(_cs);
            conn.Open();
            using var cmd = new NpgsqlCommand(
                @"UPDATE map SET
                    name         = @n,
                    rows         = @r,
                    columns      = @c,
                    description  = @d,
                    modifieddate = @md
                  WHERE id = @i", conn);
            cmd.Parameters.AddWithValue("n", mapModel.Name);
            cmd.Parameters.AddWithValue("r", mapModel.Rows);
            cmd.Parameters.AddWithValue("c", mapModel.Columns);
            cmd.Parameters.AddWithValue("d", (object?)mapModel.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("md", mapModel.ModifiedDate);
            cmd.Parameters.AddWithValue("i", id);
            cmd.ExecuteNonQuery();
        }

        public bool DeleteMap(int id)
        {
            using var conn = new NpgsqlConnection(_cs);
            conn.Open();
            using var cmd = new NpgsqlCommand(
                @"DELETE FROM map WHERE id = @i", conn);
            cmd.Parameters.AddWithValue("i", id);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool CheckCoordinate(int id, int x, int y)
        {
            using var conn = new NpgsqlConnection(_cs);
            conn.Open();
            using var cmd = new NpgsqlCommand(
                // returns TRUE if 0 ≤ x < columns AND 0 ≤ y < rows
                @"SELECT 
                    (@x BETWEEN 0 AND columns - 1)
                 AND(@y BETWEEN 0 AND rows - 1)
                  FROM map
                  WHERE id = @i", conn);
            cmd.Parameters.AddWithValue("i", id);
            cmd.Parameters.AddWithValue("x", x);
            cmd.Parameters.AddWithValue("y", y);
            return (bool)cmd.ExecuteScalar()!;
        }
    }
}
