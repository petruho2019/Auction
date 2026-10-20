using Api.Common.Data.Interfaces;
using Identity.Application.Interfaces;
using Identity.Data.Dtos;

namespace Api.Endpoints.Auth;

public sealed class RegisterEndpoint : IEndpoint
{
    public void Register(IEndpointRouteBuilder endpointsBuilder)
    {
        endpointsBuilder.MapPost("register", Handle);
    }

    private static async Task<IResult> Handle(
        RegisterRequestDto request,
        IAuthenticationService authenticationService,
        CancellationToken cancellationToken)
    {
        if (!IsValidEmail(request.Email))
            return Results.BadRequest(new { error = "A valid email address is required." });
        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
            return Results.BadRequest(new { error = "Password must contain at least 8 characters." });
        if (request.Password.Length > 128)
            return Results.BadRequest(new { error = "Password cannot exceed 128 characters." });

        var user = await authenticationService.RegisterAsync(request, cancellationToken);
        if (user is null)
            return Results.Conflict(new { error = "An account with this email already exists." });

        return Results.Created(
            $"/api/v1/identity/me",
            user);
    }

    private static bool IsValidEmail(string? email) =>
        !string.IsNullOrWhiteSpace(email)
        && email.Length <= 320
        && new System.ComponentModel.DataAnnotations.EmailAddressAttribute()
            .IsValid(email.Trim());
}
