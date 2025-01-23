using Microsoft.EntityFrameworkCore;

namespace ApiBet.Models
{
  [Index(nameof(Email), IsUnique = true)]
  [Index(nameof(PhoneNumber), IsUnique = true)]
  public class User
  {
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }

    public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
    public ICollection<GroupInvitation> Invitations { get; set; } = new List<GroupInvitation>();
    public List<Bet> Bets { get; set; } = new List<Bet>();
  }

}
