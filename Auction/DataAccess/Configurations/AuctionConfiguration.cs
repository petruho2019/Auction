using AuctionApi.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuctionApi.DataAccess.Configurations;

public sealed class AuctionConfiguration : IEntityTypeConfiguration<Auction>
{
    public void Configure(EntityTypeBuilder<Auction> builder)
    {
        builder.HasKey(auction => auction.Id);
        builder.Property(auction => auction.ProductName).HasMaxLength(200).IsRequired();
        builder.Property(auction => auction.StartsAt).IsRequired();
        builder.Property(auction => auction.EndsAt).IsRequired();
        builder.Property(auction => auction.PhotoPath).HasMaxLength(500);
        builder.Property(auction => auction.CreatedAt).IsRequired();
    }
}
