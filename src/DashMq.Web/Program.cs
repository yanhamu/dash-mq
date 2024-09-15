using DashMq.Web.Features.Datapoints;
using DashMq.Web.Infrastructure;
using MQTTnet;
using MQTTnet.Client;

namespace DashMq.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews();

        builder.Services.AddTransient<IReadDatapointHandler, ReadDatapointHandler>();

        builder.Services.AddSingleton<IMqttClient>(_ => new MqttFactory().CreateMqttClient());
        builder.Services.AddHostedService<SubscriberService>();

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}