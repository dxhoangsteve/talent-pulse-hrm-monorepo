using BaseSource.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseSource.Data.Configurations
{
    public class LeaveRequestConfiguration : IEntityTypeConfiguration<LeaveRequest>
    {
        public void Configure(EntityTypeBuilder<LeaveRequest> builder)
        {
            builder.ToTable("LeaveRequests");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasMaxLength(128);
            
            builder.Property(x => x.LeaveType).HasConversion<byte>();
            builder.Property(x => x.Status).HasConversion<byte>();
            builder.Property(x => x.TotalDays).HasPrecision(5, 1);
            builder.Property(x => x.Reason).HasMaxLength(500);
            builder.Property(x => x.RejectReason).HasMaxLength(500);
            builder.Property(x => x.ApprovedBy).HasMaxLength(128);

            // Employee relationship
            builder.HasOne(x => x.Employee)
                .WithMany(e => e.LeaveRequests)
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(x => x.EmployeeId);
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => new { x.StartDate, x.EndDate });
        }
    }
}
