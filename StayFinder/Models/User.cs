namespace StayFinder.Models;

// Model koji predstavlja korisnika u sistemu — moze biti Owner ili Guest
public sealed class User
{
    // MongoDB koristi string ID umesto int, generisemo ga kao GUID
    public string? Id { get; set; }
    public string Email { get; set; } = string.Empty;
    // Nikad ne cuvamo plain password — uvek samo hash!
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    // Uloga odredjuje sta korisnik sme da radi: "Owner" ili "Guest"
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
