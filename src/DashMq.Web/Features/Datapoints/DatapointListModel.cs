using DashMq.DataAccess.Model;

namespace DashMq.Web.Features.Datapoints;

public class DatapointListModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Topic { get; set; }
    public Direction Direction { get; set; }
    public DatapointValueModel LastValue { get; set; }
}