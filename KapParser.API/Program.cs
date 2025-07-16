using DatabaseService;
using Microsoft.EntityFrameworkCore;
using Hangfire;
using KapParser.API.Helpers;
using Hangfire.Redis.StackExchange;
using Worker.Jobs;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Veritabaný baðlantýsý
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
);

// Hangfire servislerini ekle
builder.Services.AddHangfire(config => config.UseRedisStorage("localhost"));
builder.Services.AddHangfireServer();

// Swagger (dokümantasyon)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Hangfire Dashboard (herkese açýk)
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new AllowAllUsersAuthorizationFilter() }
});

app.UseAuthorization();
app.MapControllers();

//RecurringJob.AddOrUpdate<KapJob>(
//    "fetch-kap",
//    job => job.FetchAndSaveCompaniesAsync(),
//   Cron.Minutely);
app.Run();