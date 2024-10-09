using DashMq.DataAccess.Model;
using Microsoft.EntityFrameworkCore;

namespace DashMq.DataAccess.Repositories;

public class DatapointValueRepository(DashDbContext context) : IDatapointValueRepository
{
    public Task<DatapointValue[]> ListAsync(int datapointId, LimitOffset limitOffset, CancellationToken cancellationToken)
    {
        return context.DatapointValues
            .Where(x => x.DatapointId == datapointId)
            .OrderByDescending(x => x.Timestamp)
            .Skip(limitOffset.Offset)
            .Take(limitOffset.Limit)
            .ToArrayAsync(cancellationToken);
    }

    public async Task AddAsync(DatapointValue value, CancellationToken cancellationToken)
    {
        context.DatapointValues.Add(value);
        await context.SaveChangesAsync(cancellationToken);
    }
}