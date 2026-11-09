using AuctionApi.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuctionApi.DataAccess.Configurations;

public sealed class AuctionParticipantConfiguration : IEntityTypeConfiguration<AuctionParticipant>
{
    public void Configure(EntityTypeBuilder<AuctionParticipant> builder)
    {
        builder.HasKey(participant => new { participant.AuctionId, participant.UserId });
        builder.HasIndex(participant => participant.UserId);
        builder.HasOne(participant => participant.Auction)
            .WithMany(auction => auction.Participants)
            .HasForeignKey(participant => participant.AuctionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
