using Entities;
using Worker.Services;
using Worker.Jobs;
using Worker;
using Hangfire;
using Hangfire.Redis.StackExchange;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Uygulama ayarları
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);


builder.Services.AddControllers(); // Web desteği için

// Hangfire Redis konfigürasyonu
builder.Services.AddHangfire(config =>
    config.UseRedisStorage("localhost:6379")); // BURASIII BAKK BURAYA
builder.Services.AddHangfireServer();

// Servisler
builder.Services.AddHttpClient(); // DatabaseService'e Http üzerinden istek atmak için
builder.Services.AddScoped<KapParseService>();
builder.Services.AddScoped<KapJob>();
builder.Services.AddScoped<TcmbService>();
builder.Services.AddScoped<TcmbJob>();
builder.Services.AddSingleton<RecurringJobs>();
// Hosted service (Worker arkada çalışacak)
builder.Services.AddHostedService<Workers>();


// Build et ve app nesnesini oluştur
var app = builder.Build();

// Routing
app.UseRouting();
app.UseAuthorization();

// Hangfire Dashboard aktif
app.UseHangfireDashboard("/hangfire");

// Controller endpoint'leri
app.MapControllers();
app.MapGet("/", () => "Worker API + Hangfire Dashboard aktif!");


// IConfiguration'ı bağımlılıktan alma
var recurringJobs = app.Services.GetRequiredService<RecurringJobs>();

// RecurringJobs örneği oluşturup ve jobları register etmek
recurringJobs.AddOrUpdate(); 

// Uygulama çalıştır
app.Run();
