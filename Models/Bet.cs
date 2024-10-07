

namespace ApiBet.Models
{
  public class Bet
  {
    public int Id { get; set; }
    public int GroupId { get; set; }
    public required Group Group { get; set; } // Relation till Group

    public int UserId { get; set; }
    public required User User { get; set; } // Relation till User

    public string BetDetails { get; set; } = string.Empty;
    public int Points { get; set; }
  }
}
