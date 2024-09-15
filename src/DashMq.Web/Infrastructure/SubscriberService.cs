using DashMq.Web.Features.Datapoints;
using Microsoft.Extensions.Caching.Memory;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;

namespace DashMq.Web.Infrastructure;

public class SubscriberService(IMqttClient mqttClient, IMemoryCache memoryCache) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var options = new MqttClientOptionsBuilder()
            .WithTcpServer("127.0.0.1")
            .Build();

        var subscriberOptions = new MqttClientSubscribeOptionsBuilder()
            .WithTopicFilter("local/test", MqttQualityOfServiceLevel.ExactlyOnce)
            .Build();

        await mqttClient.ConnectAsync(options, cancellationToken);
        mqttClient.ApplicationMessageReceivedAsync += MessageReceivedAsync;

        await mqttClient.SubscribeAsync(subscriberOptions, cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await mqttClient.DisconnectAsync(cancellationToken: cancellationToken);
    }

    private async Task MessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
    {
        // TODO use smarter deserializer

        var measurement = Convert.ToDouble(arg.ApplicationMessage.ConvertPayloadToString());
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var result = await memoryCache.GetOrCreateAsync<DatapointModel>("measurement", entry =>
        {
            // Set cache options (optional)
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            entry.SlidingExpiration = TimeSpan.FromMinutes(2);

            var record = CreateRecord();
            entry.Value = record;
            return Task.FromResult(record);
        });

        result!.Values.Add(new DatapointValueModel() { Value = measurement, Timestamp = timestamp });
    }

    private DatapointModel CreateRecord()
    {
        return new DatapointModel
        {
            Id = 1,
            Name = "test",
            Values = []
        };
    }
}