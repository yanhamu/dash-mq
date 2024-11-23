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

    public async Task<IEnumerable<(int, double, long)>> GetAsync(int[] toArray, CancellationToken cancellationToken)
    {
        var result = new List<(int, double, long)>(toArray.Length);

        foreach (var datapointId in toArray)
        {
            var value = await context.DatapointValues
                .Where(x => x.DatapointId == datapointId)
                .Select(x => new { x.DatapointId, x.Value, x.Timestamp })
                .OrderByDescending(x => x.Timestamp)
                .Take(1)
                .FirstOrDefaultAsync(cancellationToken);

            if (value != null)
            {
                result.Add((value.DatapointId, value.Value, value.Timestamp));
            }
        }

        return result;
    }

    public async Task AddAsync(DatapointValue value, CancellationToken cancellationToken)
    {
        context.DatapointValues.Add(value);
        await context.SaveChangesAsync(cancellationToken);
    }
}