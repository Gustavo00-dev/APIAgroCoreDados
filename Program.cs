using APIAgroCoreDados.Data;
using APIAgroCoreDados.Services;
using Microsoft.EntityFrameworkCore;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseMySql(connectionString, Microsoft.EntityFrameworkCore.ServerVersion.AutoDetect(connectionString));
});

builder.Services.AddSingleton<RabbitMqService>();
builder.Services.AddHostedService<RabbitMqListener>();

var app = builder.Build();

app.UseHttpMetrics();

app.MapMetrics();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
