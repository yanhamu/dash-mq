using MQTTnet;
using MQTTnet.Client;

namespace DashMq.Subscriber.Cli;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Connecting to broker");
        var cancellationTokenSource = new CancellationTokenSource();

        using var client = new MqttFactory().CreateMqttClient();
        var options = new MqttClientOptionsBuilder()
            .WithTcpServer("127.0.0.1")
            .Build();

        var subscribeOptions = new MqttClientSubscribeOptionsBuilder().WithTopicFilter("local/test").Build();
        await client.ConnectAsync(options, cancellationTokenSource.Token);
        await client.SubscribeAsync(subscribeOptions, cancellationTokenSource.Token);

        client.ApplicationMessageReceivedAsync += e =>
        {
            Console.WriteLine(e.ApplicationMessage.ConvertPayloadToString());
            return Task.CompletedTask;
        };

        Console.WriteLine("Hit any key to exit...");
        Console.ReadKey();
    }
}