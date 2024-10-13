using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DashMq.DataAccess.Model;

public class Datapoint
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Topic { get; set; } = default!;
    public Direction Direction { get; set; }
}

public enum Direction
{
    Invalid = 0,
    In,
    Out
}

public class DataPointMap : IEntityTypeConfiguration<Datapoint>
{
    public void Configure(EntityTypeBuilder<Datapoint> builder)
    {
        builder.ToTable("datapoints");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(400)
            .IsRequired(false);

        builder.Property(x => x.Topic)
            .HasColumnName("topic")
            .HasMaxLength(400)
            .IsRequired(false);

        builder.Property(x => x.Direction)
            .HasColumnName("direction")
            .IsRequired();
    }
}