using ApiBet.Data;
using ApiBet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiBet.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class UsersController : ControllerBase
  {
    private readonly BettingContext _context;
    private readonly IConfiguration _configuration;

    public UsersController(BettingContext context, IConfiguration configuration)
    {
      _context = context;
      _configuration = configuration;
    }

    public class UserDto
    {
      public int Id { get; set; }
      public string Username { get; set; }
      public string Email { get; set; }
      public string? PhoneNumber { get; set; }
    }

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
      public string? PhoneNumber { get; set; }
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

      var token = GenerateJwtToken(foundUser);
      return Ok(new { Token = token, User = new UserDto { Id = foundUser.Id, Username = foundUser.Username, Email = foundUser.Email, PhoneNumber = foundUser.PhoneNumber } });
    }

    private string GenerateJwtToken(User user)
    {
      var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
      var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

      var claims = new[]
      {
        new Claim(JwtRegisteredClaimNames.Sub, user.Username),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
      };

      var token = new JwtSecurityToken(
          issuer: _configuration["Jwt:Issuer"],
          audience: _configuration["Jwt:Audience"],
          claims: claims,
          expires: DateTime.Now.AddMinutes(30),
          signingCredentials: credentials);

      return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
      var users = await _context.Users
          .Select(u => new UserDto { Id = u.Id, Username = u.Username, Email = u.Email, PhoneNumber = u.PhoneNumber })
          .ToListAsync();
      return Ok(users);
    }

    [Authorize]
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
      public string? PhoneNumber { get; set; }
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
      var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

      if (string.IsNullOrEmpty(token))
      {
        return BadRequest("Ingen token skickades.");
      }

      // Svartlista token
      var blacklistedToken = new BlacklistedToken
      {
        Token = token,
        ExpiryDate = DateTime.UtcNow.AddMinutes(120) // Ange tokenens giltighetstid
      };

      _context.BlacklistedTokens.Add(blacklistedToken);
      await _context.SaveChangesAsync();

      return Ok("Token har blivit svartlistad.");
    }

    [Authorize]
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

      if (!string.IsNullOrEmpty(request.PhoneNumber))
      {
        user.PhoneNumber = request.PhoneNumber;
      }

      _context.Users.Update(user);
      await _context.SaveChangesAsync();

      return Ok(new UserDto { Id = user.Id, Username = user.Username, Email = user.Email, PhoneNumber = user.PhoneNumber });
    }

    [Authorize]
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
