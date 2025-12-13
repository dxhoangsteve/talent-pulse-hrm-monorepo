using BaseSource.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseSource.Data.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("Departments");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasMaxLength(128);
            
            // builder.Property(x => x.Code).IsRequired().HasMaxLength(20);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Description).HasMaxLength(500);
            // builder.Property(x => x.IsActive).HasDefaultValue(true);

            // Main relationship: Department -> Users
            builder.HasMany(x => x.Users)
                .WithOne(u => u.Department)
                .HasForeignKey(u => u.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            // Manager relationship
            builder.HasOne(x => x.Manager)
                .WithMany()
                .HasForeignKey(x => x.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Deputy relationship
            builder.HasOne(x => x.Deputy)
                .WithMany()
                .HasForeignKey(x => x.DeputyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            // builder.HasIndex(x => x.Code).IsUnique();
            // builder.HasIndex(x => x.IsActive);
        }
    }
}
