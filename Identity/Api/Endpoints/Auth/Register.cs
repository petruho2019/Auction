using Api.Common.Data.Interfaces;
using Data.Dtos;
using Identity.Application.Interfaces;

namespace Api.Endpoints.Auth;

public sealed class RegisterEndpoint : IEndpoint
{
    public void Register(IEndpointRouteBuilder endpointsBuilder)
    {
        endpointsBuilder.MapPost("auth/register", Handle);
    }

    private static async Task<IResult> Handle(
        RegisterRequestDto request,
        IAuthenticationService authenticationService,
        CancellationToken cancellationToken)
    {
        if (!IsValidEmail(request.Email))
            return Results.BadRequest(new { error = "A valid email address is required." });
        if (!IsValidUsername(request.Username))
            return Results.BadRequest(new
            {
                error = "Username must be between 3 and 50 characters and contain only letters, numbers, dots, underscores, or hyphens."
            });
        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
            return Results.BadRequest(new { error = "Password must contain at least 8 characters." });
        if (request.Password.Length > 128)
            return Results.BadRequest(new { error = "Password cannot exceed 128 characters." });

        var user = await authenticationService.RegisterAsync(request, cancellationToken);
        if (user is null)
            return Results.Conflict(new { error = "An account with this email or username already exists." });

        return Results.Created(
            $"/api/v1/identity/me",
            user);
    }

    private static bool IsValidEmail(string? email) =>
        !string.IsNullOrWhiteSpace(email)
        && email.Length <= 320
        && new System.ComponentModel.DataAnnotations.EmailAddressAttribute()
            .IsValid(email.Trim());

    private static bool IsValidUsername(string? username) =>
        !string.IsNullOrWhiteSpace(username)
        && username.Trim().Length is >= 3 and <= 50
        && username.Trim().All(character =>
            char.IsLetterOrDigit(character)
            || character is '.' or '_' or '-');
}
