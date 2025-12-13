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
            
            // GPS fields
            builder.Property(x => x.CheckInLatitude);
            builder.Property(x => x.CheckInLongitude);
            builder.Property(x => x.CheckInAccuracy);
            builder.Property(x => x.CheckOutLatitude);
            builder.Property(x => x.CheckOutLongitude);
            builder.Property(x => x.CheckOutAccuracy);
            builder.Property(x => x.IsMockedLocation).HasDefaultValue(false);

            // Employee relationship
            builder.HasOne(x => x.Employee)
                .WithMany(e => e.Attendances)
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes - Unique per employee per day
            builder.HasIndex(x => new { x.EmployeeId, x.Date }).IsUnique();
            builder.HasIndex(x => x.Date);
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.IsMockedLocation);
        }
    }
}

