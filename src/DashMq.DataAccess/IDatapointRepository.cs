using DashMq.DataAccess.Model;

namespace DashMq.DataAccess;

public interface IDatapointRepository
{
    Task<Datapoint?> GetAsync(int id, CancellationToken cancellationToken);
    
    Task<Datapoint[]> ListAsync(CancellationToken cancellationToken);
}