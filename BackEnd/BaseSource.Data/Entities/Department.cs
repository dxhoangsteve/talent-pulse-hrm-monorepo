using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSource.Data.Entities
{
    [Table("Departments")]
    public class Department
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        // Leadership
        public string? ManagerId { get; set; }
        [ForeignKey("ManagerId")]
        public virtual AppUser? Manager { get; set; }

        public string? DeputyId { get; set; }
        [ForeignKey("DeputyId")]
        public virtual AppUser? Deputy { get; set; }

        // Navigation
        public virtual ICollection<AppUser> Users { get; set; } = new List<AppUser>();

        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
    }
}
