using DashMq.DataAccess.Model;

namespace DashMq.Web.Features.Datapoints;

public class DatapointModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public Direction Direction { get; set; }
    public List<DatapointValueModel> Values { get; set; } = [];
}

public class DatapointValueModel
{
    public long Timestamp { get; set; }
    public double Value { get; set; }
}