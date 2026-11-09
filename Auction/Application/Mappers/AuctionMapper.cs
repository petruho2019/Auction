using AuctionApi.Data.Dtos;
using AuctionApi.Data.Entities;

namespace AuctionApi.Application.Mappers;

public static class AuctionMapper
{
    public static Auction ToEntity(CreateAuctionRequest request, string? photoPath) =>
        new()
        {
            ProductName = request.ProductName!.Trim(),
            StartsAt = request.StartsAt!.Value,
            EndsAt = request.EndsAt!.Value,
            PhotoPath = photoPath
        };

    public static AuctionResponse ToResponse(Auction auction) =>
        new(
            auction.Id,
            auction.ProductName,
            auction.StartsAt,
            auction.EndsAt,
            auction.PhotoPath,
            auction.CreatedAt);
}
