// Models/UserGroup.cs
namespace ApiBet.Models
{
  public class UserGroup
  {
    public int UserId { get; set; }
    public User User { get; set; } // Relation till User

    public int GroupId { get; set; }
    public required Group Group { get; set; } // Relation till Group

    public string Role { get; set; } = string.Empty; // T.ex. Admin, Member, etc.
  }
}
