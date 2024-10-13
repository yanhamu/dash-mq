using System.Text.Json;
using DashMq.DataAccess;
using DashMq.DataAccess.Model;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;

namespace DashMq.Web.Infrastructure;

public class SubscriberService(
    IServiceProvider services,
    IMqttClient mqttClient,
    MqttBrokerConfiguration config) : IHostedService
{
    private Dictionary<string, int> datapointsMap = new();

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var optionsBuilder = new MqttClientOptionsBuilder()
            .WithTcpServer(config.TcpHost);

        if (!string.IsNullOrWhiteSpace(config.Password))
        {
            optionsBuilder.WithCredentials("", config.Password);
        }

        var options = optionsBuilder.Build();

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
        
        var xxx = await mqttClient.SubscribeAsync(subscriberOptions, cancellationToken);
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

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var measurement = JsonSerializer.Deserialize<Message>(arg.ApplicationMessage.ConvertPayloadToString(), options);
        if (measurement == null)
            return;

        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        var value = new DatapointValue { Value = measurement.Value, Timestamp = timestamp, DatapointId = datapointId.Value };

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

public class Message
{
    public double Value { get; set; }
}