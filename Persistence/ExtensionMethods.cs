using System;
using System.Linq;
using FastMember;
using Npgsql;

namespace robot_controller_api.Persistence
{
    /// <summary>
    /// Extension methods for ADO data mapping
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Maps the current row of an NpgsqlDataReader onto an entity instance using FastMember
        /// </summary>
        public static void MapTo<T>(this NpgsqlDataReader dr, T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // Create a FastMember accessor for the entity type
            var accessor = TypeAccessor.Create(entity.GetType());

            // Collect all property names (case-insensitive)
            var props = accessor.GetMembers()
                                .Select(m => m.Name)
                                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            // For each column in the reader, find a matching property and set it
            for (int i = 0; i < dr.FieldCount; i++)
            {
                var columnName = dr.GetName(i);
                var propName = props
                    .FirstOrDefault(p => p.Equals(columnName, StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrEmpty(propName))
                {
                    var value = dr.IsDBNull(i) ? null : dr.GetValue(i);
                    accessor[entity, propName] = value;
                }
            }
        }
    }
}
