using System.Security.Authentication;
using System.Security.Claims;
using JobTracker.Api.Models;
using Microsoft.AspNetCore.Authentication;

namespace JobTracker.Api.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _context;

    public long UserId
    {
        get
        {
            if (!IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Unauthorized!");
            }

            return int.Parse(_context.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }
    }

    public UserRole Role
    {
        get
        {
            if (!IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Unauthorized!");
            }

            string role = _context.HttpContext!.User.FindFirstValue(ClaimTypes.Role)!;

            return (UserRole) Enum.Parse(typeof(UserRole), role);
        }
    }

    public bool IsAuthenticated
    {
        get
        {
            var identity = _context.HttpContext!.User.Identity;

            if (identity == null)
            {
                throw new UnauthorizedAccessException("Unauthorized!");
            }

            return identity.IsAuthenticated;
        }
    }

    public CurrentUserService(IHttpContextAccessor context)
    {
        _context = context;
    }

    
}