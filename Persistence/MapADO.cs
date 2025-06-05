#if false
using System.Collections.Generic;
using Npgsql;
using robot_controller_api.Models;

namespace robot_controller_api.Persistence
{
    public class MapADO : IMapDataAccess
    {
        private const string CONNECTION_STRING =
            "Host=localhost;Username=postgres;Password=Lash_SIT331;Database=sit331";

        public List<Map> GetMaps()
        {
            var list = new List<Map>();
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();
            using var cmd = new NpgsqlCommand(
                "SELECT id, columns, rows, name, description, createddate, modifieddate FROM map",
                conn);
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                list.Add(new Map(
                    id: dr.GetInt32(0),
                    columns: dr.GetInt32(1),
                    rows: dr.GetInt32(2),
                    name: dr.GetString(3),
                    description: dr.IsDBNull(4) ? null : dr.GetString(4),
                    createdDate: dr.GetDateTime(5),
                    modifiedDate: dr.GetDateTime(6)
                ));
            }
            return list;
        }

        public Map? GetMapById(int id)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();
            using var cmd = new NpgsqlCommand(
                "SELECT id, columns, rows, name, description, createddate, modifieddate " +
                "FROM map WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("id", id);
            using var dr = cmd.ExecuteReader();
            if (!dr.Read()) return null;
            return new Map(
                id: dr.GetInt32(0),
                columns: dr.GetInt32(1),
                rows: dr.GetInt32(2),
                name: dr.GetString(3),
                description: dr.IsDBNull(4) ? null : dr.GetString(4),
                createdDate: dr.GetDateTime(5),
                modifiedDate: dr.GetDateTime(6)
            );
        }

        public Map AddMap(Map newMap)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();
            using var cmd = new NpgsqlCommand(
                "INSERT INTO map (columns, rows, name, description, createddate, modifieddate) " +
                "VALUES (@cols, @rows, @name, @desc, NOW(), NOW()) " +
                "RETURNING id, createddate, modifieddate;", conn);
            cmd.Parameters.AddWithValue("cols", newMap.Columns);
            cmd.Parameters.AddWithValue("rows", newMap.Rows);
            cmd.Parameters.AddWithValue("name", newMap.Name);
            cmd.Parameters.AddWithValue("desc", (object?)newMap.Description ?? DBNull.Value);
            using var dr = cmd.ExecuteReader();
            dr.Read();
            newMap.Id = dr.GetInt32(0);
            newMap.CreatedDate = dr.GetDateTime(1);
            newMap.ModifiedDate = dr.GetDateTime(2);
            return newMap;
        }

        public bool UpdateMap(Map updatedMap)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();
            using var cmd = new NpgsqlCommand(
                "UPDATE map " +
                "SET columns=@cols, rows=@rows, name=@name, description=@desc, modifieddate=NOW() " +
                "WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("id", updatedMap.Id);
            cmd.Parameters.AddWithValue("cols", updatedMap.Columns);
            cmd.Parameters.AddWithValue("rows", updatedMap.Rows);
            cmd.Parameters.AddWithValue("name", updatedMap.Name);
            cmd.Parameters.AddWithValue("desc", (object?)updatedMap.Description ?? DBNull.Value);
            return cmd.ExecuteNonQuery() == 1;
        }

        public bool DeleteMap(int id)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();
            using var cmd = new NpgsqlCommand(
                "DELETE FROM map WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("id", id);
            return cmd.ExecuteNonQuery() == 1;
        }
    }
}
#endif