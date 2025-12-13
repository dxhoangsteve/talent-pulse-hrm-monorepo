using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSource.Data.Entities
{
    /// <summary>
    /// Tài khoản người dùng hệ thống
    /// </summary>
    public class AppUser : IdentityUser<string>
    {
        public string FullName { get; set; } = string.Empty;
        public string? Avatar { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedTime { get; set; }
        public DateTime? DeletedTime { get; set; }


        public decimal BaseSalary { get; set; } = 0;
        
        public BaseSource.Shared.Enums.PositionType Position { get; set; } = BaseSource.Shared.Enums.PositionType.None;

        public Guid? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual Department? Department { get; set; }

        // Navigation
        public Employee? Employee { get; set; }
        public ICollection<AppUserRole> UserRoles { get; set; } = new List<AppUserRole>();
        public ICollection<AppUserClaim> Claims { get; set; } = new List<AppUserClaim>();
        public ICollection<AppUserToken> Tokens { get; set; } = new List<AppUserToken>();
        public ICollection<AppUserLogin> AppUserLogins { get; set; } = new List<AppUserLogin>();
    }
}
