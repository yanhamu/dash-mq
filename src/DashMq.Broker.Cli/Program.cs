using MQTTnet;
using MQTTnet.Server;

namespace DashMq.Broker.Cli;

class Program
{
    static async Task Main(string[] args)
    {
        var mqttServerFactory = new MqttFactory();

        // The port for the default endpoint is 1883.
        // The default endpoint is NOT encrypted!
        // Use the builder classes where possible.
        var mqttServerOptions = new MqttServerOptionsBuilder().WithDefaultEndpoint().Build();

        // The port can be changed using the following API (not used in this example).
        // new MqttServerOptionsBuilder()
        //     .WithDefaultEndpoint()
        //     .WithDefaultEndpointPort(1234)
        //     .Build();

        using var mqttServer = mqttServerFactory.CreateMqttServer(mqttServerOptions);

        mqttServer.InterceptingPublishAsync += OnPublish;
        await mqttServer.StartAsync();
        Console.WriteLine("MqttServer is running...");

        Console.WriteLine("Press Enter to exit.");
        Console.ReadLine();

        // Stop and dispose the MQTT server if it is no longer needed!
        await mqttServer.StopAsync();
    }

    private static Task OnPublish(InterceptingPublishEventArgs arg)
    {
        Console.WriteLine("received message");
        return Task.CompletedTask;
    }
}