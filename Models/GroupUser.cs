namespace ApiBet.Models
{
  public class UserGroup
  {
    public int UserId { get; set; }
    public User User { get; set; }

    public int GroupId { get; set; }
    public Group Group { get; set; }

    public string Role { get; set; } = "Member"; // Standardrollen är "Member", men kan vara "Admin"
  }
}
