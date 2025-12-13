using BaseSource.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseSource.Data.Configurations
{
    public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
    {
        public void Configure(EntityTypeBuilder<Attendance> builder)
        {
            builder.ToTable("Attendances");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasMaxLength(128);
            
            builder.Property(x => x.Status).HasConversion<byte>();
            builder.Property(x => x.WorkHours).HasPrecision(5, 2);
            builder.Property(x => x.OvertimeHours).HasPrecision(5, 2);
            builder.Property(x => x.Note).HasMaxLength(500);
            builder.Property(x => x.CheckInLocation).HasMaxLength(200);

            // Employee relationship
            builder.HasOne(x => x.Employee)
                .WithMany(e => e.Attendances)
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes - Unique per employee per day
            builder.HasIndex(x => new { x.EmployeeId, x.Date }).IsUnique();
            builder.HasIndex(x => x.Date);
            builder.HasIndex(x => x.Status);
        }
    }
}
