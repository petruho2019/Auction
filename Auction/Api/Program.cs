using AuctionApi.Application;
using AuctionApi.DataAccess;
using Api.Common.Extensions;
using RabbitMq.Common;

namespace AuctionApi;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var services = builder.Services;
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is not configured.");

        services.AddOpenApi();
        services.AddValidation();
        services.AddDataAccess(connectionString);
        services.AddApplication();
        services.AddRabbitMq(
            builder.Configuration.GetSection("RabbitMq"),
            "auction",
            typeof(Program).Assembly,
            typeof(AuctionApi.Application.DependencyInjection).Assembly);
        services.AddCors(options =>
        {
            options.AddPolicy(
                "AuctionClient",
                policy => policy
                    .WithOrigins("http://localhost:4200")
                    .AllowAnyHeader()
                    .AllowAnyMethod());
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseCors("AuctionClient");
        app.UseStaticFiles();
        app.MapGroup("/api/v1/auctions").MapEndpoints();

        await using (var scope = app.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AuctionDbContext>();
            await dbContext.Database.EnsureCreatedAsync();
        }

        await app.RunAsync();
    }
}
