using Identity.Data.Dtos;

namespace Data.Dtos;

public sealed record AuthenticationResultDto(UserDto User, string? RenewedAccessToken);
