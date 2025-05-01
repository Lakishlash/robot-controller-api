using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql;
using robot_controller_api.Models;

namespace robot_controller_api.Persistence
{
    public class MapRepository : IMapDataAccess, IRepository
    {
        private IRepository _repo => this;

        public List<Map> GetMaps()
            => _repo.ExecuteReader<Map>(
                "SELECT id, columns, rows, name, description, createddate, modifieddate FROM map");

        public Map? GetMapById(int id)
        {
            var list = _repo.ExecuteReader<Map>(
                "SELECT id, columns, rows, name, description, createddate, modifieddate " +
                "FROM map WHERE id = @id",
                new[] { new NpgsqlParameter("id", id) });
            return list.SingleOrDefault();
        }

        public Map AddMap(Map newMap)
        {
            var sqlParams = new[]
            {
                new NpgsqlParameter("columns", newMap.Columns),
                new NpgsqlParameter("rows", newMap.Rows),
                new NpgsqlParameter("name", newMap.Name),
                new NpgsqlParameter("description", (object?)newMap.Description ?? DBNull.Value)
            };

            var created = _repo.ExecuteReader<Map>(
                "INSERT INTO map (columns, rows, name, description, createddate, modifieddate) " +
                "VALUES (@columns, @rows, @name, @description, NOW(), NOW()) " +
                "RETURNING id, columns, rows, name, description, createddate, modifieddate;",
                sqlParams)
                .Single();

            return created;
        }

        public bool UpdateMap(Map updatedMap)
        {
            var sqlParams = new[]
            {
                new NpgsqlParameter("id", updatedMap.Id),
                new NpgsqlParameter("columns", updatedMap.Columns),
                new NpgsqlParameter("rows", updatedMap.Rows),
                new NpgsqlParameter("name", updatedMap.Name),
                new NpgsqlParameter("description", (object?)updatedMap.Description ?? DBNull.Value)
            };

            var updated = _repo.ExecuteReader<Map>(
                "UPDATE map SET columns=@columns, rows=@rows, name=@name, description=@description, modifieddate=NOW() " +
                "WHERE id=@id RETURNING id;",
                sqlParams);

            return updated.Any();
        }

        public bool DeleteMap(int id)
        {
            var deleted = _repo.ExecuteReader<Map>(
                "DELETE FROM map WHERE id=@id RETURNING id;",
                new[] { new NpgsqlParameter("id", id) });

            return deleted.Any();
        }
    }
}
