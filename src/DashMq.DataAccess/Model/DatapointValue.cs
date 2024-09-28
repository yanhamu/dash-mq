using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DashMq.DataAccess.Model;

public class DatapointValue
{
    public long Id { get; set; }
    public int DatapointId { get; set; }
    public long Timestamp { get; set; }
    public double Value { get; set; }

    public Datapoint Datapoint { get; set; } = default!;
}

public class DatapointValueMap : IEntityTypeConfiguration<DatapointValue>
{
    public void Configure(EntityTypeBuilder<DatapointValue> builder)
    {
        builder.ToTable("datapoint_values");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.DatapointId)
            .HasColumnName("datapoint_id")
            .IsRequired();

        builder.Property(x => x.Timestamp)
            .HasColumnName("timestamp")
            .IsRequired();

        builder.Property(x => x.Value)
            .HasColumnName("value")
            .IsRequired();

        builder.HasOne(x => x.Datapoint)
            .WithMany()
            .HasForeignKey(x => x.DatapointId);
    }
}