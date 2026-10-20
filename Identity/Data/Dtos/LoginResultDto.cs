namespace Identity.Data.Dtos;

public sealed record LoginResultDto(
    UserDto User,
    string AccessToken,
    string RefreshToken);
