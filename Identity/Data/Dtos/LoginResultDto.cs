using Identity.Data.Dtos;

namespace Data.Dtos;

public sealed record LoginResultDto(
    UserDto User,
    string AccessToken,
    string RefreshToken);
