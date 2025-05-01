using System;

namespace robot_controller_api.Models
{
    public class Map
    {
        public int Id { get; set; }
        public int Columns { get; set; }
        public int Rows { get; set; }

        
        public string Name { get; set; } = default!;

        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        // parameterless ctor for ORM
        public Map() { }

        public Map(
            int id,
            int columns,
            int rows,
            string name,
            string? description,
            DateTime createdDate,
            DateTime modifiedDate)
        {
            Id = id;
            Columns = columns;
            Rows = rows;
            Name = name;
            Description = description;
            CreatedDate = createdDate;
            ModifiedDate = modifiedDate;
        }
    }
}
