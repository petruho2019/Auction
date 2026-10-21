using System.Security.Claims;
using System.Text.Encodings.Web;
using Identity.Application.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Identity.Api.Auth;

public sealed class AccessRefreshAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    Application.Interfaces.IAuthenticationService authenticationService)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var accessToken = Request.Cookies[AuthTokenCookies.AccessTokenName];
        var refreshToken = Request.Cookies[AuthTokenCookies.RefreshTokenName];
        if (string.IsNullOrWhiteSpace(accessToken) && string.IsNullOrWhiteSpace(refreshToken))
            return AuthenticateResult.NoResult();

        var result = await authenticationService.AuthenticateAsync(
            accessToken,
            refreshToken,
            Context.RequestAborted);
        if (result is null)
            return AuthenticateResult.Fail("Unauthorized"); //"Refresh token is invalid or expired."

        if (result.RenewedAccessToken is not null)
            AuthTokenCookies.AppendAccess(Response, result.RenewedAccessToken);

        return Success(result.User);
    }

    private AuthenticateResult Success(Data.Dtos.UserDto user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()!),
            new Claim(ClaimTypes.Name, user.Username!),
            new Claim(ClaimTypes.Email, user.Email!)
        };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
    }
}
