// Models/Map.cs
using System;

namespace robot_controller_api.Models
{
    public class Map
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Rows { get; set; }
        public int Columns { get; set; }
        public string? Description { get; set; }        // ← add this
        public bool IsSquare { get; set; }        // matches your GENERATED ALWAYS column
        public DateTime CreatedDate { get; set; }        // maps to createddate
        public DateTime ModifiedDate { get; set; }        // maps to modifieddate
    }
}
