namespace DashMq.Web.Features.Datapoints;

public class DatapointModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<DatapointValueModel> Values { get; set; } = new();
}

public class DatapointValueModel
{
    public long Timestamp { get; set; }
    public double Value { get; set; }
}