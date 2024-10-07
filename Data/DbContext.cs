// Data/BettingContext.cs
using Microsoft.EntityFrameworkCore;
using ApiBet.Models;

namespace ApiBet.Data
{
  public class BettingContext : DbContext
  {
    public BettingContext(DbContextOptions<BettingContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<UserGroup> UserGroups { get; set; }
    public DbSet<Bet> Bets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<UserGroup>()
          .HasKey(ug => new { ug.UserId, ug.GroupId });

      modelBuilder.Entity<UserGroup>()
          .HasOne(ug => ug.User)
          .WithMany(u => u.UserGroups)
          .HasForeignKey(ug => ug.UserId);

      modelBuilder.Entity<UserGroup>()
          .HasOne(ug => ug.Group)
          .WithMany(g => g.UserGroups)
          .HasForeignKey(ug => ug.GroupId);
    }
  }
}
