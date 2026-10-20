namespace Identity.Data;

public static class TokenLifetimes
{
    public static readonly TimeSpan AccessToken = TimeSpan.FromMinutes(15);
    public static readonly TimeSpan RefreshToken = TimeSpan.FromDays(30);
}
