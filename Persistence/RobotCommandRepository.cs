using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql;
using robot_controller_api.Models;

namespace robot_controller_api.Persistence
{
    public class RobotCommandRepository : IRobotCommandDataAccess, IRepository
    {
        
        private IRepository _repo => this;

        public List<RobotCommand> GetRobotCommands()
            => _repo.ExecuteReader<RobotCommand>(
                "SELECT id, name, description, ismovecommand, createddate, modifieddate FROM robotcommand");

        public RobotCommand? GetRobotCommandById(int id)
        {
            var list = _repo.ExecuteReader<RobotCommand>(
                "SELECT id, name, description, ismovecommand, createddate, modifieddate " +
                "FROM robotcommand WHERE id = @id",
                new[] { new NpgsqlParameter("id", id) });
            return list.SingleOrDefault();
        }

        public RobotCommand AddRobotCommand(RobotCommand newCmd)
        {
            var sqlParams = new[]
            {
                new NpgsqlParameter("name", newCmd.Name),
                new NpgsqlParameter("description", (object?)newCmd.Description ?? DBNull.Value),
                new NpgsqlParameter("ismovecommand", newCmd.IsMoveCommand)
            };

            var created = _repo.ExecuteReader<RobotCommand>(
                "INSERT INTO robotcommand (name, description, ismovecommand, createddate, modifieddate) " +
                "VALUES (@name, @description, @ismovecommand, NOW(), NOW()) " +
                "RETURNING id, name, description, ismovecommand, createddate, modifieddate;",
                sqlParams)
                .Single();

            return created;
        }

        public bool UpdateRobotCommand(RobotCommand cmdToUpdate)
        {
            var sqlParams = new[]
            {
                new NpgsqlParameter("id", cmdToUpdate.Id),
                new NpgsqlParameter("name", cmdToUpdate.Name),
                new NpgsqlParameter("description", (object?)cmdToUpdate.Description ?? DBNull.Value),
                new NpgsqlParameter("ismovecommand", cmdToUpdate.IsMoveCommand)
            };

            var updated = _repo.ExecuteReader<RobotCommand>(
                "UPDATE robotcommand SET " +
                "name=@name, description=@description, ismovecommand=@ismovecommand, modifieddate=NOW() " +
                "WHERE id=@id RETURNING id;",
                sqlParams);

            return updated.Any();
        }

        public bool DeleteRobotCommand(int id)
        {
            var deleted = _repo.ExecuteReader<RobotCommand>(
                "DELETE FROM robotcommand WHERE id=@id RETURNING id;",
                new[] { new NpgsqlParameter("id", id) });

            return deleted.Any();
        }
    }
}
