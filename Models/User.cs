namespace ApiBet.Models
{
  public class User
  {
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; } // Valfritt fält

    // Relationer
    public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>(); // Relation till grupper
    public ICollection<GroupInvitation> Invitations { get; set; } = new List<GroupInvitation>(); // Relation till inbjudningar
    public List<Bet> Bets { get; set; } = new List<Bet>(); // Lägg till denna för relationen med `Bets`
  }
}
