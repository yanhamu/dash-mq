namespace DashMq.Web.Infrastructure;

public class MqttBrokerConfiguration
{
    public string TcpHost { get; init; } = default!;
    public string Password { get; init; } = default!;
}