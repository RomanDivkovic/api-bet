namespace ApiBet.Models
{
  public class Group
  {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; // Initialiserat för att undvika null
    public List<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
    public List<Bet> Bets { get; set; } = new List<Bet>(); // Om Bets används
    public List<GroupInvitation> Invitations { get; set; } = new List<GroupInvitation>(); // Lägg till detta om relevant
  }
}
