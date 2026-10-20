using Identity.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.DataAccess;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(user => user.Id);
            entity.Property(user => user.Email).HasMaxLength(320).IsRequired();
            entity.HasIndex(user => user.Email).IsUnique();
            entity.Property(user => user.PasswordHash).IsRequired();
            entity.Property(user => user.Role).HasMaxLength(50).IsRequired();
            entity.Property(user => user.CreatedAt).IsRequired();
        });

        modelBuilder.Entity<UserSession>(entity =>
        {
            entity.HasKey(session => session.Id);
            entity.Property(session => session.AccessTokenHash).HasMaxLength(64).IsRequired();
            entity.Property(session => session.RefreshTokenHash).HasMaxLength(64).IsRequired();
            entity.HasIndex(session => session.AccessTokenHash).IsUnique();
            entity.HasIndex(session => session.RefreshTokenHash).IsUnique();
            entity.HasIndex(session => session.UserId);
            entity.HasOne(session => session.User)
                .WithMany(user => user.Sessions)
                .HasForeignKey(session => session.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
