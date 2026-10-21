using Data.Dtos;
using Identity.Data.Dtos;

namespace Identity.Application.Interfaces;

public interface IAuthenticationService
{
    Task<UserDto?> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken);

    Task<LoginResultDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken);

    Task<AuthenticationResultDto?> AuthenticateAsync(
        string? accessToken,
        string? refreshToken,
        CancellationToken cancellationToken);

    Task RevokeAsync(
        string? accessToken,
        string? refreshToken,
        CancellationToken cancellationToken);
}
