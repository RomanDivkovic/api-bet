using Microsoft.AspNetCore.Http;
using ApiBet.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


public class TokenBlackListMiddlewear
{
  private readonly RequestDelegate _next;

  public TokenBlackListMiddlewear(RequestDelegate next)
  {
    _next = next;
  }

  public async Task InvokeAsync(HttpContext context, BettingContext dbContext)
  {
    var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

    if (!string.IsNullOrEmpty(token))
    {
      // Kontrollera om token är svartlistad
      var isBlacklisted = await dbContext.BlacklistedTokens.AnyAsync(bt => bt.Token == token);
      if (isBlacklisted)
      {
        context.Response.StatusCode = 401; // Unauthorized
        await context.Response.WriteAsync("Token är ogiltigt.");
        return;
      }
    }

    await _next(context);
  }
}
