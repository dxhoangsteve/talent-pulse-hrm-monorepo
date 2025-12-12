using System;
using System.Collections.Generic;

namespace BaseSource.Data.Entities
{
    public class Department
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? ManagerId { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedTime { get; set; }

        // Navigation
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
