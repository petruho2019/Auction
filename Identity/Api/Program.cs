

using Api.Common.Extensions;
using Identity.Application;
using Identity.Api.Auth;
using Identity.DataAccess;
using Microsoft.AspNetCore.Authentication;
using Identity.Data.Entities;

namespace Identity.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var configuration = builder.Configuration;
        var services = builder.Services;

        var connectionString = configuration.GetConnectionString("IdentityDatabase")
            ?? throw new InvalidOperationException(
                "Connection string 'IdentityDatabase' is not configured.");

        services.AddOpenApi("auction", opts =>
        {
            opts.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0;

        });

        services.AddDataAccess(connectionString);
        services.AddApplication();
        services
            .AddAuthentication("AccessRefresh")
            .AddScheme<AuthenticationSchemeOptions, AccessRefreshAuthenticationHandler>(
                "AccessRefresh",
                _ => { });
        services.AddAuthorization();
        services.AddHealthChecks();

        var app = builder.Build();


        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        var mainGroup = app.MapGroup("/api/v1/identity");

        mainGroup.MapEndpoints();

        app.UseHealthChecks("/api/v1/identity/health"); // TODO посмотреть есть ли в nginx возможности из location / перенести запрос на location /api/v1/identity при health

        app.Run();
    }

}
