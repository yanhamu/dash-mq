using DashMq.DataAccess.Model;
using Microsoft.EntityFrameworkCore;

namespace DashMq.DataAccess.Repositories;

public class DatapointRepository(DashDbContext context) : IDatapointRepository
{
    public async Task<Datapoint?> GetAsync(int id, CancellationToken cancellationToken)
    {
        return await context.Datapoints.FindAsync([id], cancellationToken: cancellationToken);
    }

    public Task<Datapoint[]> ListAsync(CancellationToken cancellationToken)
    {
        return context.Datapoints.ToArrayAsync(cancellationToken);
    }
}