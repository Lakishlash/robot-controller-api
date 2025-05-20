#if false
using System.Collections.Generic;
using Npgsql;
using robot_controller_api.Models;

namespace robot_controller_api.Persistence
{
    public class RobotCommandADO : IRobotCommandDataAccess
    {
        private const string CONNECTION_STRING =
            "Host=localhost;Username=postgres;Password=Lash_SIT331;Database=sit331";

        public List<RobotCommand> GetRobotCommands()
        {
            var list = new List<RobotCommand>();
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();
            using var cmd = new NpgsqlCommand(
                "SELECT id, name, description, ismovecommand, createddate, modifieddate FROM robotcommand",
                conn);
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                list.Add(new RobotCommand(
                    id: dr.GetInt32(0),
                    name: dr.GetString(1),
                    isMoveCommand: dr.GetBoolean(3),
                    createdDate: dr.GetDateTime(4),
                    modifiedDate: dr.GetDateTime(5),
                    description: dr.IsDBNull(2) ? null : dr.GetString(2)
                ));
            }
            return list;
        }

        public RobotCommand? GetRobotCommandById(int id)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();
            using var cmd = new NpgsqlCommand(
                "SELECT id, name, description, ismovecommand, createddate, modifieddate " +
                "FROM robotcommand WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("id", id);
            using var dr = cmd.ExecuteReader();
            if (!dr.Read()) return null;
            return new RobotCommand(
                id: dr.GetInt32(0),
                name: dr.GetString(1),
                isMoveCommand: dr.GetBoolean(3),
                createdDate: dr.GetDateTime(4),
                modifiedDate: dr.GetDateTime(5),
                description: dr.IsDBNull(2) ? null : dr.GetString(2)
            );
        }

        public RobotCommand AddRobotCommand(RobotCommand newCmd)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();
            using var cmd = new NpgsqlCommand(
                "INSERT INTO robotcommand (name, description, ismovecommand, createddate, modifieddate) " +
                "VALUES (@name, @desc, @move, NOW(), NOW()) " +
                "RETURNING id, createddate, modifieddate;", conn);
            cmd.Parameters.AddWithValue("name", newCmd.Name);
            cmd.Parameters.AddWithValue("desc", (object?)newCmd.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("move", newCmd.IsMoveCommand);
            using var dr = cmd.ExecuteReader();
            dr.Read();
            newCmd.Id = dr.GetInt32(0);
            newCmd.CreatedDate = dr.GetDateTime(1);
            newCmd.ModifiedDate = dr.GetDateTime(2);
            return newCmd;
        }

        public bool UpdateRobotCommand(RobotCommand cmdToUpdate)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();
            using var cmd = new NpgsqlCommand(
                "UPDATE robotcommand " +
                "SET name=@name, description=@desc, ismovecommand=@move, modifieddate=NOW() " +
                "WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("id", cmdToUpdate.Id);
            cmd.Parameters.AddWithValue("name", cmdToUpdate.Name);
            cmd.Parameters.AddWithValue("desc", (object?)cmdToUpdate.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("move", cmdToUpdate.IsMoveCommand);
            return cmd.ExecuteNonQuery() == 1;
        }

        public bool DeleteRobotCommand(int id)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();
            using var cmd = new NpgsqlCommand(
                "DELETE FROM robotcommand WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("id", id);
            return cmd.ExecuteNonQuery() == 1;
        }
    }
}
#endif
