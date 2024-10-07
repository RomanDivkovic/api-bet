// Models/Group.cs
namespace ApiBet.Models
{
  public class Group
  {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public List<UserGroup>? UserGroups { get; set; } // Relation till UserGroup
    public List<Bet>? Bets { get; set; } // Relation till Bets
  }
}
