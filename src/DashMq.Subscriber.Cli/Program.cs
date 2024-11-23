using Microsoft.Extensions.Configuration;
using MQTTnet;
using MQTTnet.Client;

namespace DashMq.Subscriber.Cli;

class Program
{
    static async Task Main(string[] args)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("config.json", optional: false);

        var config = builder.Build();

        Console.WriteLine("Connecting to broker");
        var cancellationTokenSource = new CancellationTokenSource();

        using var client = new MqttFactory().CreateMqttClient();
        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(config["tcpServerHost"])
            
            .Build();

        var subscribeOptions = new MqttClientSubscribeOptionsBuilder().WithTopicFilter("temperature").Build();
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