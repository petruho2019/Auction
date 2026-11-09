using AuctionApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuctionApi.DataAccess;

public sealed class AuctionDbContext(DbContextOptions<AuctionDbContext> options)
    : DbContext(options)
{
    public DbSet<Auction> Auctions => Set<Auction>();
    public DbSet<AuctionParticipant> AuctionParticipants => Set<AuctionParticipant>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuctionDbContext).Assembly);
}
