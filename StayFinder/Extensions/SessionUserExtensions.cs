using Microsoft.AspNetCore.Http;

namespace StayFinder.Extensions;

public static class SessionUserExtensions
{
    public static string? GetCurrentUserId(this HttpContext httpContext)
    {
        return httpContext.Session.GetString("UserId");
    }

    public static string? GetCurrentRole(this HttpContext httpContext)
    {
        return httpContext.Session.GetString("Role");
    }

    public static bool IsLoggedIn(this HttpContext httpContext)
    {
        return !string.IsNullOrWhiteSpace(httpContext.GetCurrentUserId());
    }

    public static bool IsInRole(this HttpContext httpContext, string role)
    {
        return string.Equals(httpContext.GetCurrentRole(), role, StringComparison.OrdinalIgnoreCase);
    }
}
