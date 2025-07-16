using Models;
using Worker.Services;
using Worker.Jobs;
using Worker;
using Hangfire;
using Hangfire.Redis.StackExchange;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using DatabaseService;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        // Veritabanı bağlantısı
        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(
                configuration.GetConnectionString("DefaultConnection"),
                ServerVersion.AutoDetect(configuration.GetConnectionString("DefaultConnection"))
            ));

        // Hangfire Redis konfigürasyonu
        services.AddHangfire(config =>
            config.UseRedisStorage("localhost:6379")); // Bağlantı adresin farklıysa değiştir

        services.AddHangfireServer();

        // Servisler
        services.AddHttpClient();
        services.AddScoped<KapParseService>();
        services.AddScoped<KapJob>();

        // Hosted service (Background Worker)
        services.AddHostedService<KapWorker>();
    });

await builder.Build().RunAsync();
