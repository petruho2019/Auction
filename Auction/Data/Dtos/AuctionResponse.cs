namespace AuctionApi.Data.Dtos;

/// <summary>Данные созданного аукциона.</summary>
public sealed record AuctionResponse(
    Guid Id,
    string ProductName,
    DateTimeOffset StartsAt,
    DateTimeOffset EndsAt,
    string? PhotoUrl,
    DateTimeOffset CreatedAt);
