using Microsoft.Extensions.Caching.Memory;

namespace DashMq.Web.Features.Datapoints;

public class ReadDatapointHandler(IMemoryCache memoryCache) : IReadDatapointHandler
{
    private readonly IMemoryCache memoryCache = memoryCache;

    public DatapointModel? Get(int id)
    {
        return memoryCache.Get<DatapointModel>("measurement");
    }
}