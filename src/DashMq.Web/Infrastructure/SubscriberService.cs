using DashMq.DataAccess;
using DashMq.DataAccess.Model;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;

namespace DashMq.Web.Infrastructure;

public class SubscriberService(
    IServiceProvider services,
    IMqttClient mqttClient) : IHostedService
{
    private Dictionary<string, int> datapointsMap = new();

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var options = new MqttClientOptionsBuilder()
            .WithTcpServer("127.0.0.1")
            .Build();

        var subscriberOptionsBuilder = new MqttClientSubscribeOptionsBuilder();

        var datapoints = await GetDatapoints(cancellationToken);
        if (!datapoints.Any())
            return;

        datapointsMap = datapoints.ToDictionary(x => x.Topic, x => x.Id);

        foreach (var datapoint in datapoints)
            subscriberOptionsBuilder = subscriberOptionsBuilder.WithTopicFilter(datapoint.Topic, MqttQualityOfServiceLevel.ExactlyOnce);

        var subscriberOptions = subscriberOptionsBuilder
            .Build();

        await mqttClient.ConnectAsync(options, cancellationToken);
        mqttClient.ApplicationMessageReceivedAsync += MessageReceivedAsync;

        await mqttClient.SubscribeAsync(subscriberOptions, cancellationToken);
    }

    private async Task<Datapoint[]> GetDatapoints(CancellationToken cancellationToken)
    {
        await using var scope = services.CreateAsyncScope();
        var datapointRepository = scope.ServiceProvider.GetRequiredService<IDatapointRepository>();
        return await datapointRepository.ListAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await mqttClient.DisconnectAsync(cancellationToken: cancellationToken);
    }

    private async Task MessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
    {
        // TODO use smarter deserializer

        var datapointId = GetDatapointId(arg.ApplicationMessage.Topic);
        if (!datapointId.HasValue)
            return;

        var measurement = Convert.ToDouble(arg.ApplicationMessage.ConvertPayloadToString());
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        var value = new DatapointValue { Value = measurement, Timestamp = timestamp, DatapointId = datapointId.Value };

        await using var scope = services.CreateAsyncScope();
        var valueRepository = scope.ServiceProvider.GetRequiredService<IDatapointValueRepository>();
        await valueRepository.AddAsync(value, default);
    }

    private int? GetDatapointId(string topic)
    {
        return datapointsMap.TryGetValue(topic, out var datapointId)
            ? datapointId
            : default(int?);
    }
}