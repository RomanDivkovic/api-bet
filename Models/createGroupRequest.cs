using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiBet.Models
{
  public class CreateGroupRequest
  {
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    public List<int> InvitedUserIds { get; set; } = new List<int>();
  }
}
