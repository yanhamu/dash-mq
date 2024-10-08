using DashMq.DataAccess.Model;

namespace DashMq.DataAccess;

public interface IDatapointValueRepository
{
    Task<DatapointValue[]> ListAsync(int datapointId, LimitOffset limitOffset, CancellationToken cancellationToken);
    Task AddAsync( DatapointValue value, CancellationToken cancellationToken);
}