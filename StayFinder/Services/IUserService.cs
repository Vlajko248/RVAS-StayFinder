using StayFinder.Models;

namespace StayFinder.Services;


// Interfejs za servis korisnika — register, login i dohvatanje korisnika
public interface IUserService
{
    // Dohvata sve korisnike — koristi se za prikaz imena umesto GUID-a
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(string id);
    // Pronalazi korisnika po emailu — email je jedinstven u sistemu
    Task<User?> GetByEmailAsync(string email);
    // Registracija — cува novog korisnika sa hashovanom lozinkom
    Task CreateAsync(User user);
    // Login — proverava email+lozinku i vraca korisnika ili null
    Task<User?> ValidateUserAsync(string email, string password);
}
