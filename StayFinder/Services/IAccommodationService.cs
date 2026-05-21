using StayFinder.Models;

namespace StayFinder.Services;

public interface IAccommodationService
{
    Task<List<Accommodation>> GetAllAsync();
    Task<List<Accommodation>> SearchByLocationAsync(string? location);
    Task<List<Accommodation>> GetByOwnerIdAsync(string ownerId);
    Task<Accommodation?> GetByIdAsync(string id);
    Task<bool> IsOwnerOfAccommodationAsync(string accommodationId, string ownerId);
    Task CreateAsync(Accommodation accommodation);
    Task UpdateAsync(string id, Accommodation accommodation);
    Task DeleteAsync(string id);
}
