using DashMq.DataAccess.Model;
using Microsoft.EntityFrameworkCore;

namespace DashMq.DataAccess.Repositories;

public class DashDbContext(DbContextOptions<DashDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DashDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Datapoint> Datapoints { get; set; }
    public DbSet<DatapointValue> DatapointValues { get; set; }
}