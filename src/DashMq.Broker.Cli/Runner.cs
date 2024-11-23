using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;

namespace DashMq.Broker.Cli;

public class Runner : BackgroundService
{
    private readonly string? password;
    private MqttServer? mqttServer;

    public Runner(IConfiguration configuration)
    {
        password = configuration["ClientConnectionPassword"];
    }

    private Task OnValidateConnection(ValidatingConnectionEventArgs arg)
    {
        Console.WriteLine("Validating connection...");

        Console.WriteLine(arg.ToString());

        if (password != null && arg.Password != password)
        {
            Console.WriteLine($"Arrived pwd {arg.Password} != {password}");
            arg.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
        }

        Console.WriteLine($"final reason: {arg.ReasonCode.ToString()}");

        return Task.CompletedTask;
    }

    private Task OnPublish(InterceptingPublishEventArgs arg)
    {
        Console.WriteLine("received message");
        Console.WriteLine($"topic alias:{arg.ApplicationMessage.TopicAlias} topic:{arg.ApplicationMessage.Topic}");
        Console.WriteLine($"message:{arg.ApplicationMessage.ConvertPayloadToString()}");
        return Task.CompletedTask;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var mqttServerFactory = new MqttFactory();

        // The port for the default endpoint is 1883.
        // The default endpoint is NOT encrypted!
        // Use the builder classes where possible.
        var mqttServerOptions = new MqttServerOptionsBuilder()
            .WithoutDefaultEndpoint()
            //.WithDefaultEndpointPort(1883)
            //.WithDefaultEndpointBoundIPAddress(IPAddress.Loopback)
            .WithDefaultEndpoint()
            .Build();

        // The port can be changed using the following API (not used in this example).
        // new MqttServerOptionsBuilder()
        //     .WithDefaultEndpoint()
        //     .WithDefaultEndpointPort(1234)
        //     .Build();

        mqttServer = mqttServerFactory.CreateMqttServer(mqttServerOptions);

        mqttServer.InterceptingPublishAsync += OnPublish;
        mqttServer.ValidatingConnectionAsync += OnValidateConnection;
        Console.WriteLine("starting MqttServer...");
        await mqttServer.StartAsync();
    }

    public override void Dispose()
    {
        Console.WriteLine("Stopping");
        mqttServer?.StopAsync().Wait();
        mqttServer?.Dispose();
    }
}