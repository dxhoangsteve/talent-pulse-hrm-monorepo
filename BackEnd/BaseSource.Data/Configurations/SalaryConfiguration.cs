using BaseSource.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseSource.Data.Configurations
{
    public class SalaryConfiguration : IEntityTypeConfiguration<Salary>
    {
        public void Configure(EntityTypeBuilder<Salary> builder)
        {
            builder.ToTable("Salaries");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.BaseSalary).HasPrecision(18, 2);
            builder.Property(x => x.OvertimePay).HasPrecision(18, 2);
            builder.Property(x => x.Bonus).HasPrecision(18, 2);
            builder.Property(x => x.Deductions).HasPrecision(18, 2);
            builder.Property(x => x.NetSalary).HasPrecision(18, 2);
            builder.Property(x => x.Status).IsRequired().HasMaxLength(20);

            builder.HasOne(x => x.Employee)
                .WithMany(e => e.Salaries)
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.EmployeeId, x.Month, x.Year }).IsUnique();
        }
    }
}
