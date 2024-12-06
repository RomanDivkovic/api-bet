namespace ApiBet.Models
{
  public class UserGroup
  {
    public int UserId { get; set; }
    public User User { get; set; }
    public int GroupId { get; set; }
    public Group Group { get; set; }
    public bool IsAdmin { get; set; } // True om användaren är admin
  }
}
