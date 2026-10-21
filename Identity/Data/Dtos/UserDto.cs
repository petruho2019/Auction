namespace Identity.Data.Dtos;

public sealed record UserDto(
    Guid? Id = null,
    string? Username = null,
    string? Email = null);
// string? Role = null);
