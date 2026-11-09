using Api.Auth;
using Api.Common.Data.Interfaces;
using Identity.Application.Interfaces;

namespace Api.Endpoints.Auth;

public sealed class LogoutEndpoint : IEndpoint
{
    public void Register(IEndpointRouteBuilder endpointsBuilder)
    {
        endpointsBuilder.MapPost("auth/logout", Handle)
            .WithName("Logout")
            .WithSummary("Выйти из аккаунта")
            .WithDescription("Отзывает текущие токены авторизации и удаляет cookies с токенами.")
            .Produces(StatusCodes.Status204NoContent);
    }

    private static async Task<IResult> Handle(
        HttpContext httpContext,
        IAuthenticationService authenticationService,
        CancellationToken cancellationToken)
    {
        await authenticationService.RevokeAsync(
            httpContext.Request.Cookies[AuthTokenCookies.AccessTokenName],
            httpContext.Request.Cookies[AuthTokenCookies.RefreshTokenName],
            cancellationToken);

        AuthTokenCookies.Clear(httpContext.Response);
        return Results.NoContent();
    }
}
