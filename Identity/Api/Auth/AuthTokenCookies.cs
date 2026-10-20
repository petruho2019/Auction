using Identity.Data;

namespace Identity.Api.Auth;

public static class AuthTokenCookies
{
    public const string AccessTokenName = "identity-access";
    public const string RefreshTokenName = "identity-refresh";

    public static void Append(HttpResponse response, string accessToken, string refreshToken)
    {
        response.Cookies.Append(AccessTokenName, accessToken, CreateOptions(TokenLifetimes.AccessToken));
        response.Cookies.Append(RefreshTokenName, refreshToken, CreateOptions(TokenLifetimes.RefreshToken));
    }

    public static void AppendAccess(HttpResponse response, string accessToken) =>
        response.Cookies.Append(AccessTokenName, accessToken, CreateOptions(TokenLifetimes.AccessToken));

    public static void Clear(HttpResponse response)
    {
        response.Cookies.Delete(AccessTokenName, CreateOptions(TimeSpan.Zero));
        response.Cookies.Delete(RefreshTokenName, CreateOptions(TimeSpan.Zero));
    }

    private static CookieOptions CreateOptions(TimeSpan maxAge) => new()
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Lax,
        Path = "/",
        MaxAge = maxAge
    };
}
