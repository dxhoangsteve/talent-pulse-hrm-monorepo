using BaseSource.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseSource.Data.Configurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employees");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasMaxLength(128);
            
            builder.Property(x => x.EmployeeCode).IsRequired().HasMaxLength(20);
            builder.Property(x => x.FullName).IsRequired().HasMaxLength(150);
            builder.Property(x => x.Email).IsRequired().HasMaxLength(256);
            builder.Property(x => x.Phone).HasMaxLength(20);
            builder.Property(x => x.Address).HasMaxLength(500);
            builder.Property(x => x.Position).HasMaxLength(100);
            
            // Enums stored as byte
            builder.Property(x => x.Level).HasConversion<byte>();
            builder.Property(x => x.Status).HasConversion<byte>();
            builder.Property(x => x.Gender).HasConversion<byte>();
            
            builder.Property(x => x.BaseSalary).HasPrecision(18, 2);
            builder.Property(x => x.RemainingLeaveDays).HasPrecision(5, 1);

            // Department relationship
            builder.HasOne(x => x.Department)
                .WithMany()
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(x => x.EmployeeCode).IsUnique();
            builder.HasIndex(x => x.Email).IsUnique();
            builder.HasIndex(x => x.DepartmentId);
            builder.HasIndex(x => x.Status);
        }
    }
}
