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

        using var mqttServer = mqttServerFactory.CreateMqttServer(mqttServerOptions);

        mqttServer.InterceptingPublishAsync += OnPublish;
        await mqttServer.StartAsync();
        Console.WriteLine("MqttServer is running...");

        var s = "";
        do
        {
            Console.WriteLine("Write 'exit' to exit.");
            s = Console.ReadLine();
        } while (s != "exit");


        // Stop and dispose the MQTT server if it is no longer needed!
        await mqttServer.StopAsync();
    }

    private static Task OnPublish(InterceptingPublishEventArgs arg)
    {
        Console.WriteLine("received message");
        Console.WriteLine($"topic alias:{arg.ApplicationMessage.TopicAlias} topic:{arg.ApplicationMessage.Topic}");
        Console.WriteLine($"message:{arg.ApplicationMessage.ConvertPayloadToString()}");
        return Task.CompletedTask;
    }
}