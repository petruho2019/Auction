using AuctionApi.Data.Dtos;

namespace AuctionApi.Application.Interfaces;

public interface IAuctionService
{
    Task<AuctionResponse> CreateAsync(
        CreateAuctionRequest request,
        CancellationToken cancellationToken);
}
