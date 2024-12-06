using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiBet.Models
{
  public enum InvitationStatus
  {
    Pending,
    Accepted,
    Declined
  }

  public class GroupInvitation
  {
    [Key]
    public int Id { get; set; }

    [Required]
    public int GroupId { get; set; }

    [ForeignKey(nameof(GroupId))]
    public Group Group { get; set; } = null!;

    [Required]
    public int InvitedUserId { get; set; }

    [ForeignKey(nameof(InvitedUserId))]
    public User InvitedUser { get; set; } = null!;

    [Required]
    public InvitationStatus Status { get; set; } = InvitationStatus.Pending;
  }
}
