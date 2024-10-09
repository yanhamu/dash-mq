using Microsoft.Extensions.Configuration;
using MQTTnet;
using MQTTnet.Client;

namespace DashMq.Publisher.Cli;

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
        await client.ConnectAsync(options, cancellationTokenSource.Token);

        const string exitKeyword = "exit";
        var input = string.Empty;

        Console.WriteLine("Type 'exit' to quit the application.");
        while (input != exitKeyword)
        {
            Console.Write("Enter message content: ");
            input = Console.ReadLine();

            if (input == exitKeyword)
            {
                Console.WriteLine("Exiting application...");
                await cancellationTokenSource.CancelAsync();
                break;
            }

            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic("local/test")
                .WithPayload(input)
                .Build();

            await client.PublishAsync(applicationMessage, cancellationTokenSource.Token);
            Console.WriteLine("Message published");
        }
    }
}