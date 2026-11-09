using Api.Auth;
using Api.Common.Data.Interfaces;
using Data.Dtos;
using Identity.Application.Interfaces;

namespace Api.Endpoints.Auth;

public class LoginEndpoint : IEndpoint
{
    public void Register(IEndpointRouteBuilder endpointsBuilder)
    {
        endpointsBuilder.MapPost("auth/login", Handle)
            .WithName("Login")
            .WithSummary("Войти в аккаунт")
            .WithDescription("Проверяет учетные данные и устанавливает cookies с токенами авторизации.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);
    }

    private static async Task<IResult> Handle(
        LoginRequestDto request,
        IAuthenticationService authenticationService,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return Results.BadRequest(new { error = "Укажите адрес электронной почты и пароль." });

        var result = await authenticationService.LoginAsync(request, cancellationToken);
        if (result is null)
            return Results.BadRequest();

        AuthTokenCookies.Append(
            httpContext.Response,
            result.AccessToken,
            result.RefreshToken);

        return Results.NoContent();
    }
}