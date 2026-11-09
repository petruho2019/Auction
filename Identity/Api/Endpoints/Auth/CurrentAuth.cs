using Api.Common.Data.Interfaces;
using Identity.Data.Dtos;

namespace Api.Endpoints.Auth;

public sealed class CurrentAuth : IEndpoint
{
    public void Register(IEndpointRouteBuilder endpointsBuilder)
    {
        endpointsBuilder.MapGet("/auth/current", Handle)
            .WithName("GetCurrentAuth")
            .WithSummary("Получить текущую авторизацию")
            .WithDescription("Возвращает данные текущего пользователя, если он авторизован.")
            .Produces<UserDto>(StatusCodes.Status200OK);
    }

    private static IResult Handle(HttpContext context)
    {
        var user = context.User;
        var id = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var username = user.FindFirst("username")?.Value;
        var email = user.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        // var role = user.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

        return id != null
            ? Results.Ok(new UserDto(Guid.Parse(id), username, email))
            : Results.Ok(new UserDto());
    }
}
