using Microsoft.AspNetCore.Mvc;
using ApiBet.Data;
using ApiBet.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiBet.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class GroupsController : ControllerBase
  {
    private readonly BettingContext _context;

    public GroupsController(BettingContext context)
    {
      _context = context;
    }

    private int GetUserIdFromToken()
    {
      // Placeholder for extracting user ID from authentication token
      return int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");
    }

    // GET: api/Groups
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Group>>> GetGroups()
    {
      return await _context.Groups
          .Include(g => g.UserGroups)
          .ThenInclude(ug => ug.User)
          .ToListAsync();
    }

    // GET: api/Groups/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Group>> GetGroup(int id)
    {
      var group = await _context.Groups
          .Include(g => g.UserGroups)
          .ThenInclude(ug => ug.User)
          .Include(g => g.Bets)
          .FirstOrDefaultAsync(g => g.Id == id);

      if (group == null)
      {
        return NotFound("Group not found.");
      }

      return group;
    }

    // POST: api/Groups/create
    [HttpPost("create")]
    public async Task<IActionResult> CreateGroup(CreateGroupRequest request)
    {
      // Get the ID of the logged-in user
      var inviterId = GetUserIdFromToken();
      var inviter = await _context.Users.FindAsync(inviterId);
      if (inviter == null)
      {
        return Unauthorized("The inviter was not found.");
      }

      // Create the group
      var group = new Group
      {
        Name = request.Name,
        UserGroups = new List<UserGroup>() // Initialize UserGroups
      };

      // Add the group to the database
      _context.Groups.Add(group);
      await _context.SaveChangesAsync();

      // Add the inviter as an admin
      var inviterUserGroup = new UserGroup
      {
        UserId = inviterId,
        GroupId = group.Id,
        IsAdmin = true
      };
      _context.UserGroups.Add(inviterUserGroup);

      // Handle invitations
      foreach (var invitedUserId in request.InvitedUserIds)
      {
        var invitation = new GroupInvitation
        {
          GroupId = group.Id,
          InvitedUserId = invitedUserId,

        };
        _context.GroupInvitations.Add(invitation);
      }

      // Save all changes
      await _context.SaveChangesAsync();

      // Load the created group with related data
      var createdGroup = await _context.Groups
          .Include(g => g.UserGroups)
          .ThenInclude(ug => ug.User)
          .Include(g => g.Invitations)
          .FirstOrDefaultAsync(g => g.Id == group.Id);

      return Ok(createdGroup);
    }

    // GET: api/Groups/{groupId}/users
    [HttpGet("{groupId}/users")]
    public async Task<IActionResult> GetUsersInGroup(int groupId)
    {
      var groupUsers = await _context.UserGroups
          .Where(ug => ug.GroupId == groupId)
          .Include(ug => ug.User)
          .Select(ug => new { ug.User.Id, ug.User.Username, ug.User.Email, ug.IsAdmin })
          .ToListAsync();

      if (!groupUsers.Any())
      {
        return NotFound("No users found in the group.");
      }

      return Ok(groupUsers);
    }

    // POST: api/Groups/{groupId}/invite
    [HttpPost("{groupId}/invite")]
    public async Task<IActionResult> InviteUserToGroup(int groupId, [FromBody] int invitedUserId)
    {
      var userId = GetUserIdFromToken();
      var inviter = await _context.UserGroups
          .FirstOrDefaultAsync(ug => ug.GroupId == groupId && ug.UserId == userId);

      if (inviter == null || !inviter.IsAdmin)
      {
        return Forbid("Only administrators can invite users.");
      }

      var group = await _context.Groups.FindAsync(groupId);
      var user = await _context.Users.FindAsync(invitedUserId);

      if (group == null || user == null)
      {
        return NotFound("Group or user not found.");
      }

      var invitation = new GroupInvitation
      {
        GroupId = groupId,
        InvitedUserId = invitedUserId,

      };

      _context.GroupInvitations.Add(invitation);
      await _context.SaveChangesAsync();

      return Ok("Invitation sent.");
    }

    [HttpPost("{groupId}/invitations/{invitationId}/accept")]
    public async Task<IActionResult> AcceptInvitation(int groupId, int invitationId)
    {
      var userId = GetUserIdFromToken();

      var invitation = await _context.GroupInvitations
          .FirstOrDefaultAsync(i => i.Id == invitationId && i.GroupId == groupId && i.InvitedUserId == userId);

      if (invitation == null)
      {
        return NotFound("Inbjudan hittades inte eller är inte riktad till dig.");
      }

      // Lägg till användaren i gruppen
      var userGroup = new UserGroup
      {
        UserId = userId,
        GroupId = groupId,
        IsAdmin = false
      };
      _context.UserGroups.Add(userGroup);

      // Ta bort inbjudan efter att den godkänts
      _context.GroupInvitations.Remove(invitation);

      await _context.SaveChangesAsync();

      return Ok("Du har gått med i gruppen.");
    }

    [HttpPost("{groupId}/invitations/{invitationId}/decline")]
    public async Task<IActionResult> DeclineInvitation(int groupId, int invitationId)
    {
      var userId = GetUserIdFromToken();

      var invitation = await _context.GroupInvitations
          .FirstOrDefaultAsync(i => i.Id == invitationId && i.GroupId == groupId && i.InvitedUserId == userId);

      if (invitation == null)
      {
        return NotFound("Inbjudan hittades inte eller är inte riktad till dig.");
      }

      // Ta bort inbjudan efter att den nekats
      _context.GroupInvitations.Remove(invitation);
      await _context.SaveChangesAsync();

      return Ok("Du har nekat inbjudan.");
    }


    // DELETE: api/Groups/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGroup(int id)
    {
      var group = await _context.Groups.FindAsync(id);
      if (group == null)
      {
        return NotFound();
      }

      _context.Groups.Remove(group);
      await _context.SaveChangesAsync();

      return NoContent();
    }
  }
}
