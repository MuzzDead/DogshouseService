using DogshouseService.BLL.Interfaces;
using DogshouseService.BLL.Services;
using DogshouseService.DAL;
using DogshouseService.DAL.Configurations;
using DogshouseService.DAL.Interfaces;
using DogshouseService.DAL.Repositories;
using DogshouseService.WebAPI.Middlewares;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<IDogsRepository, DogsRepository>();
builder.Services.AddScoped<IDogService, DogService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        await DbInitializer.SeedAsync(services, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database seeding failed.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<SimpleRateLimitMiddleware>(builder.Configuration.GetValue<int>("RateLimit:RequestsPerSecond"));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
