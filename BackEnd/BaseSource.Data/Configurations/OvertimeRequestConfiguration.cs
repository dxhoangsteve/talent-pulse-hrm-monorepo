using BaseSource.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseSource.Data.Configurations
{
    public class OvertimeRequestConfiguration : IEntityTypeConfiguration<OvertimeRequest>
    {
        public void Configure(EntityTypeBuilder<OvertimeRequest> builder)
        {
            builder.ToTable("OvertimeRequests");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Hours).HasPrecision(5, 2);
            builder.Property(x => x.Reason).HasMaxLength(500);
            builder.Property(x => x.Status).IsRequired().HasMaxLength(20);

            builder.HasOne(x => x.Employee)
                .WithMany(e => e.OvertimeRequests)
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.EmployeeId, x.OTDate });
        }
    }
}
