using Api.Common.Data.Interfaces;
using Identity.Api.Auth;
using Identity.Application.Interfaces;
using Identity.Data.Dtos;

namespace Api.Endpoints.Auth;

public class LoginEndpoint : IEndpoint
{
    public void Register(IEndpointRouteBuilder endpointsBuilder)
    {
        endpointsBuilder.MapPost("login", Handle);
    }

    private static async Task<IResult> Handle(
        LoginRequestDto request,
        IAuthenticationService authenticationService,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return Results.BadRequest(new { error = "Email and password are required." });

        var result = await authenticationService.LoginAsync(request, cancellationToken);
        if (result is null)
            return Results.Unauthorized();

        AuthTokenCookies.Append(
            httpContext.Response,
            result.AccessToken,
            result.RefreshToken);

        return Results.NoContent();
    }
}