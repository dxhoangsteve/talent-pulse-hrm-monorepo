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
            builder.Property(x => x.Id).HasMaxLength(128);
            
            builder.Property(x => x.Status).HasConversion<byte>();
            builder.Property(x => x.BaseSalary).HasPrecision(18, 2);
            builder.Property(x => x.OvertimePay).HasPrecision(18, 2);
            builder.Property(x => x.Bonus).HasPrecision(18, 2);
            builder.Property(x => x.Allowance).HasPrecision(18, 2);
            builder.Property(x => x.Deductions).HasPrecision(18, 2);
            builder.Property(x => x.Insurance).HasPrecision(18, 2);
            builder.Property(x => x.Tax).HasPrecision(18, 2);
            builder.Property(x => x.NetSalary).HasPrecision(18, 2);
            builder.Property(x => x.ActualWorkDays).HasPrecision(5, 2);
            builder.Property(x => x.Note).HasMaxLength(500);
            builder.Property(x => x.ApprovedBy).HasMaxLength(128);
            builder.Property(x => x.PaidBy).HasMaxLength(128);

            // Employee relationship
            builder.HasOne(x => x.Employee)
                .WithMany(e => e.Salaries)
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Approver relationship
            builder.HasOne(x => x.ApprovedByUser)
                .WithMany()
                .HasForeignKey(x => x.ApprovedBy)
                .OnDelete(DeleteBehavior.NoAction);

            // Payer relationship
            builder.HasOne(x => x.PaidByUser)
                .WithMany()
                .HasForeignKey(x => x.PaidBy)
                .OnDelete(DeleteBehavior.NoAction);

            // Unique per employee per month per year
            builder.HasIndex(x => new { x.EmployeeId, x.Month, x.Year }).IsUnique();
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.PaidTime);
        }
    }
}

