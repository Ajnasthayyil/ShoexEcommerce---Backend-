using ShoexEcommerce.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShoexEcommerce.Infrastructure.Data;
using System.Security.Claims;

public class BlockedUserMiddleware
{
    private readonly RequestDelegate _next;

    public BlockedUserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, AppDbContext db)
    {
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = context.User.Claims
                .FirstOrDefault(c => c.Type == "userid")?.Value;

            if (int.TryParse(userIdClaim, out var userId))
            {
                var user = await db.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == userId);

                if (user == null || !user.IsActive || user.IsBlocked)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Account is blocked or inactive.");
                    return;
                }
            }
        }

        await _next(context);
    }
}
