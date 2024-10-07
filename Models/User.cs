// Models/User.cs
using System.ComponentModel.DataAnnotations;

namespace ApiBet.Models
{
  public class User
  {
    public int Id { get; set; }

    public string? PhoneNumber { get; set; } = string.Empty;

    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public List<UserGroup>? UserGroups { get; set; } // Relation till UserGroup
    public string? Email { get; internal set; }
  }
}
