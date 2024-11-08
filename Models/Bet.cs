using ApiBet.Models;

public class Bet
{
  public int Id { get; set; }
  public int UserId { get; set; }
  public User User { get; set; }
  public int GroupId { get; set; }
  public Group Group { get; set; }
  public decimal Amount { get; set; }
  public string Description { get; set; }
  public string Result { get; set; }
  public string Status { get; set; }
}
