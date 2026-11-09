namespace AuctionApi.Data.Entities;

public sealed class AuctionParticipant
{
    public Guid AuctionId { get; set; }
    public Auction Auction { get; set; } = null!;
    public Guid UserId { get; set; }
}
