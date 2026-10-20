using Identity.Application.Interfaces;
using Identity.Data;
using Identity.Data.Dtos;
using Identity.Data.Entities;
using Identity.DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Identity.Application.Services;

public sealed class AuthenticationService(
    ApplicationDbContext dbContext,
    IPasswordHasher<User> passwordHasher) : IAuthenticationService
{
    public async Task<UserDto?> RegisterAsync(
        RegisterRequestDto request,
        CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        if (await dbContext.Users.AnyAsync(user => user.Email == email, cancellationToken))
            return null;

        var user = new User { Email = email };
        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
        dbContext.Users.Add(user);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
            when (exception.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.UniqueViolation
            })
        {
            return null;
        }

        return ToDto(user);
    }

    public async Task<LoginResultDto?> LoginAsync(
        LoginRequestDto request,
        CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await dbContext.Users.SingleOrDefaultAsync(
            candidate => candidate.Email == email,
            cancellationToken);

        if (user is null
            || passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password)
                == PasswordVerificationResult.Failed)
        {
            return null;
        }

        var now = DateTimeOffset.UtcNow;
        var accessToken = OpaqueToken.Generate();
        var refreshToken = OpaqueToken.Generate();
        dbContext.UserSessions.Add(new UserSession
        {
            UserId = user.Id,
            User = user,
            AccessTokenHash = OpaqueToken.Hash(accessToken),
            RefreshTokenHash = OpaqueToken.Hash(refreshToken),
            AccessExpiresAt = now.Add(TokenLifetimes.AccessToken),
            RefreshExpiresAt = now.Add(TokenLifetimes.RefreshToken)
        });
        await dbContext.SaveChangesAsync(cancellationToken);

        return new LoginResultDto(ToDto(user), accessToken, refreshToken);
    }

    public async Task<AuthenticationResultDto?> AuthenticateAsync(
        string? accessToken,
        string? refreshToken,
        CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            var accessHash = OpaqueToken.Hash(accessToken);
            var session = await dbContext.UserSessions
                .Include(candidate => candidate.User)
                .SingleOrDefaultAsync(
                    candidate => candidate.AccessTokenHash == accessHash
                        && candidate.RevokedAt == null,
                    cancellationToken);

            if (session is not null && session.AccessExpiresAt > now)
                return new AuthenticationResultDto(ToDto(session.User), null);
        }

        if (string.IsNullOrWhiteSpace(refreshToken))
            return null;

        var refreshHash = OpaqueToken.Hash(refreshToken);
        var refreshSession = await dbContext.UserSessions
            .Include(candidate => candidate.User)
            .SingleOrDefaultAsync(
                candidate => candidate.RefreshTokenHash == refreshHash
                    && candidate.RevokedAt == null,
                cancellationToken);

        if (refreshSession is null || refreshSession.RefreshExpiresAt <= now)
            return null;

        var renewedAccessToken = OpaqueToken.Generate();
        refreshSession.AccessTokenHash = OpaqueToken.Hash(renewedAccessToken);
        refreshSession.AccessExpiresAt = now.Add(TokenLifetimes.AccessToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new AuthenticationResultDto(ToDto(refreshSession.User), renewedAccessToken);
    }

    public async Task RevokeAsync(
        string? accessToken,
        string? refreshToken,
        CancellationToken cancellationToken)
    {
        var accessHash = string.IsNullOrWhiteSpace(accessToken)
            ? null
            : OpaqueToken.Hash(accessToken);
        var refreshHash = string.IsNullOrWhiteSpace(refreshToken)
            ? null
            : OpaqueToken.Hash(refreshToken);

        if (accessHash is null && refreshHash is null)
            return;

        var sessions = await dbContext.UserSessions
            .Where(
                candidate => (accessHash != null && candidate.AccessTokenHash == accessHash)
                    || (refreshHash != null && candidate.RefreshTokenHash == refreshHash))
            .ToListAsync(cancellationToken);

        foreach (var session in sessions)
            session.RevokedAt = DateTimeOffset.UtcNow;

        if (sessions.Count > 0)
            await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static UserDto ToDto(User user) => new(user.Id, user.Email, user.Role);
}
