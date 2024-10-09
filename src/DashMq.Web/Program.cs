using DashMq.DataAccess;
using DashMq.DataAccess.Repositories;
using DashMq.Web.Infrastructure;
using Microsoft.EntityFrameworkCore;
using MQTTnet;
using MQTTnet.Client;

namespace DashMq.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews();

        builder.Services.AddTransient<IDatapointRepository, DatapointRepository>();
        builder.Services.AddTransient<IDatapointValueRepository, DatapointValueRepository>();

        builder.Services.AddSingleton<IMqttClient>(_ => new MqttFactory().CreateMqttClient());
        builder.Services.AddHostedService<SubscriberService>();

        var mqttConfig = builder.Configuration.GetSection("MqttBroker").Get<MqttBrokerConfiguration>();

        builder.Services.AddSingleton(mqttConfig!);
        
        builder.Services.AddDbContext<DashDbContext>(options =>
        {
            options.UseSqlite(connectionString: builder.Configuration.GetConnectionString("DefaultConnection"));
        });

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