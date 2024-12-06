using Microsoft.EntityFrameworkCore;
using ApiBet.Models;

namespace ApiBet.Data
{
  public class BettingContext : DbContext
  {
    public DbSet<User> Users { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Bet> Bets { get; set; }
    public DbSet<UserGroup> UserGroups { get; set; }
    public DbSet<GroupInvitation> GroupInvitations { get; set; }
    public DbSet<BlacklistedToken> BlacklistedTokens { get; set; }

    public BettingContext(DbContextOptions<BettingContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      // Konfigurera relationer och tabeller
      modelBuilder.Entity<UserGroup>()
          .HasKey(ug => new { ug.GroupId, ug.UserId });

      modelBuilder.Entity<UserGroup>()
          .HasOne(ug => ug.User)
          .WithMany(u => u.UserGroups)
          .HasForeignKey(ug => ug.UserId);

      modelBuilder.Entity<UserGroup>()
          .HasOne(ug => ug.Group)
          .WithMany(g => g.UserGroups)
          .HasForeignKey(ug => ug.GroupId);

      modelBuilder.Entity<GroupInvitation>()
          .HasOne(gi => gi.Group)
          .WithMany(g => g.Invitations)
          .HasForeignKey(gi => gi.GroupId);

      modelBuilder.Entity<GroupInvitation>()
          .HasOne(gi => gi.InvitedUser)
          .WithMany()
          .HasForeignKey(gi => gi.InvitedUserId);

      modelBuilder.Entity<Bet>()
          .HasOne(b => b.User)
          .WithMany(u => u.Bets)
          .HasForeignKey(b => b.UserId);

      modelBuilder.Entity<Bet>()
          .HasOne(b => b.Group)
          .WithMany(g => g.Bets)
          .HasForeignKey(b => b.GroupId);

      // Konfiguration för svartlistade tokens
      modelBuilder.Entity<BlacklistedToken>()
          .Property(bt => bt.Token)
          .IsRequired();

      modelBuilder.Entity<BlacklistedToken>()
          .Property(bt => bt.ExpiryDate)
          .IsRequired();
    }
  }
}
