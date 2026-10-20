namespace Identity.Data.Dtos;

public sealed record AuthenticationResultDto(UserDto User, string? RenewedAccessToken);
