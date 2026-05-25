using StayFinder.Models;

namespace StayFinder.Services;

// Interfejs za servis smestaja — definise sta servis mora da ume da uradi
// Implementacija je u AccommodationService.cs
public interface IAccommodationService
{
    Task<List<Accommodation>> GetAllAsync();
    // Pretraga po lokaciji — ako je location null, vraca sve smestaje
    Task<List<Accommodation>> SearchByLocationAsync(string? location);
    Task<List<Accommodation>> GetByOwnerIdAsync(string ownerId);
    Task<Accommodation?> GetByIdAsync(string id);
    // Provera da li je dati korisnik vlasnik smestaja — za autorizaciju
    Task<bool> IsOwnerOfAccommodationAsync(string accommodationId, string ownerId);
    Task CreateAsync(Accommodation accommodation);
    Task UpdateAsync(string id, Accommodation accommodation);
    Task DeleteAsync(string id);
}
