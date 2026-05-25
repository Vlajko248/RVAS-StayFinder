using Microsoft.AspNetCore.Http;

namespace StayFinder.Extensions;

// Extension metode za HttpContext — koristimo ih u svim kontrolerima
// umesto da svuda rucno pisemo Session.GetString("UserId")
public static class SessionUserExtensions
{
    // Vraca ID ulogovanog korisnika iz sesije (ili null ako nije ulogovan)
    public static string? GetCurrentUserId(this HttpContext httpContext)
    {
        return httpContext.Session.GetString("UserId");
    }

    // Vraca ulogu korisnika: "Owner" ili "Guest"
    public static string? GetCurrentRole(this HttpContext httpContext)
    {
        return httpContext.Session.GetString("Role");
    }

    // Brza provera da li je korisnik ulogovan
    public static bool IsLoggedIn(this HttpContext httpContext)
    {
        return !string.IsNullOrWhiteSpace(httpContext.GetCurrentUserId());
    }

    // Provera uloge — koristi se za zastitu ruta (npr. samo Owner sme da kreira smestaj)
    public static bool IsInRole(this HttpContext httpContext, string role)
    {
        return string.Equals(httpContext.GetCurrentRole(), role, StringComparison.OrdinalIgnoreCase);
    }
}
