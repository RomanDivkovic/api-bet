using Microsoft.AspNetCore.Mvc;
using ApiBet.Data;
using ApiBet.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ApiBet.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class UsersController : ControllerBase
  {
    private readonly BettingContext _context;

    public UsersController(BettingContext context)
    {
      _context = context;
    }

    // DTO för att returnera användare utan att exponera lösenordshash
    public class UserDto
    {
      public int Id { get; set; }
      public string Username { get; set; }
      public string Email { get; set; }
      public string? PhoneNumber { get; set; } // Telefonnummer är valfritt
    }

    // Register Request
    public class RegisterRequest
    {
      [Required]
      public string Username { get; set; }

      [Required]
      [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
          ErrorMessage = "Lösenordet måste innehålla minst en stor bokstav, en liten bokstav, en siffra, ett specialtecken och vara minst 6 tecken långt.")]
      public string Password { get; set; }

      [Required]
      [EmailAddress]
      public string Email { get; set; }

      [Phone]
      public string? PhoneNumber { get; set; } // Telefon är valfritt
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var user = new User
      {
        Username = request.Username,
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
        Email = request.Email,
        PhoneNumber = request.PhoneNumber
      };

      _context.Users.Add(user);
      await _context.SaveChangesAsync();
      return Ok(new UserDto { Id = user.Id, Username = user.Username, Email = user.Email, PhoneNumber = user.PhoneNumber });
    }

    public class LoginRequest
    {
      [Required]
      public string Username { get; set; }

      [Required]
      public string Password { get; set; }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
      var foundUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
      if (foundUser == null || !BCrypt.Net.BCrypt.Verify(request.Password, foundUser.PasswordHash))
        return Unauthorized("Fel användarnamn eller lösenord.");

      return Ok(new UserDto { Id = foundUser.Id, Username = foundUser.Username, Email = foundUser.Email, PhoneNumber = foundUser.PhoneNumber });
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
      var users = await _context.Users
          .Select(u => new UserDto { Id = u.Id, Username = u.Username, Email = u.Email, PhoneNumber = u.PhoneNumber })
          .ToListAsync();
      return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
      var user = await _context.Users
          .Where(u => u.Id == id)
          .Select(u => new UserDto { Id = u.Id, Username = u.Username, Email = u.Email, PhoneNumber = u.PhoneNumber })
          .FirstOrDefaultAsync();

      if (user == null)
      {
        return NotFound("Användaren hittades inte.");
      }

      return Ok(user);
    }

    public class UpdateUserRequest
    {
      [Required]
      public string Username { get; set; }

      [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
          ErrorMessage = "Lösenordet måste innehålla minst en stor bokstav, en liten bokstav, en siffra, ett specialtecken och vara minst 6 tecken långt.")]
      public string? Password { get; set; }

      [EmailAddress]
      public string? Email { get; set; }

      [Phone]
      public string? PhoneNumber { get; set; } // Telefon är valfritt
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
    {
      var user = await _context.Users.FindAsync(id);
      if (user == null)
      {
        return NotFound("Användaren hittades inte.");
      }

      user.Username = request.Username;

      if (!string.IsNullOrEmpty(request.Password))
      {
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
      }

      if (!string.IsNullOrEmpty(request.Email))
      {
        user.Email = request.Email;
      }

      if (!string.IsNullOrEmpty(request.Password))
      {
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
      }

      if (!string.IsNullOrEmpty(request.PhoneNumber))
      {
        user.PhoneNumber = request.PhoneNumber;
      }

      _context.Users.Update(user);
      await _context.SaveChangesAsync();

      return Ok(new UserDto { Id = user.Id, Username = user.Username, Email = user.Email, PhoneNumber = user.PhoneNumber });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
      var user = await _context.Users.FindAsync(id);
      if (user == null)
      {
        return NotFound("Användaren hittades inte.");
      }

      _context.Users.Remove(user);
      await _context.SaveChangesAsync();

      return Ok($"Användare med ID {id} raderades.");
    }
  }
}
