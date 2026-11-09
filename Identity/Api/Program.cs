

using Api.Common.Extensions;
using Identity.Application;
using Identity.DataAccess;
using Microsoft.AspNetCore.Authentication;
using Api.Auth;
using RabbitMq.Common;

namespace Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var configuration = builder.Configuration;
        var services = builder.Services;

        services.AddValidation();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is not configured.");

        services.AddOpenApi("auction", opts =>
        {
            opts.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0;
        });

        services.AddDataAccess(connectionString);
        services.AddApplication();
        services.AddRabbitMq(
            configuration.GetSection("RabbitMq"),
            "identity",
            typeof(Program).Assembly,
            typeof(Identity.Application.DependencyInjection).Assembly);
        services
            .AddAuthentication("AccessRefresh")
            .AddScheme<AuthenticationSchemeOptions, AccessRefreshAuthenticationHandler>(
                "AccessRefresh",
                _ => { });
        services.AddAuthorization();
        services.AddHealthChecks();

        services.AddCors(opts =>
        {
            opts.AddPolicy("AllowAllDevFront", policy =>
            {
                policy.WithOrigins("http://localhost:4200")
                 .AllowAnyHeader()
                 .AllowAnyMethod()
                 .AllowCredentials();
            });
        });

        var app = builder.Build();

        app.UseCors("AllowAllDevFront");

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        var mainGroup = app.MapGroup("/api/v1/identity");

        mainGroup.MapEndpoints();

        app.UseHealthChecks("/api/v1/identity/health"); // TODO посмотреть есть ли в nginx возможности из location / перенести запрос на location /api/v1/identity при health

        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.EnsureCreatedAsync();
        }
        app.Run();
    }
}
