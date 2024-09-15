namespace DashMq.Web.Features.Datapoints;

public interface IReadDatapointHandler
{
    DatapointModel? Get(int id);
}