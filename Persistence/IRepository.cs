using System.Collections.Generic;
using System.Linq;
using Npgsql;
using FastMember;

namespace robot_controller_api.Persistence
{
    public interface IRepository
    {
        /// <summary>
        /// Executes the given SQL command and maps each row into a new T via FastMember
        /// </summary>
        List<T> ExecuteReader<T>(string sqlCommand, NpgsqlParameter[]? dbParams = null)
            where T : class, new()
        {
            var entities = new List<T>();
            const string CONNECTION_STRING =
                "Host=localhost;Username=postgres;Password=Lash_SIT331;Database=sit331";

            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();

            using var cmd = new NpgsqlCommand(sqlCommand, conn);
            if (dbParams is not null)
                cmd.Parameters.AddRange(dbParams.Where(x => x.Value is not null).ToArray());

            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                var entity = new T();
                dr.MapTo(entity);
                entities.Add(entity);
            }

            return entities;
        }
    }
}
