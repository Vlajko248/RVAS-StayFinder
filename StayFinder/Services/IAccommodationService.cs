using StayFinder.Models;

namespace StayFinder.Services;

public interface IAccommodationService
{
    // TODO: Add accommodation CRUD and search operations.
    //nastavak
    Task<List<Accommodation>> GetAllAsync();

    Task<Accommodation?> GetByIdAsync(string id);

    Task CreateAsync(Accommodation accommodation);

    Task UpdateAsync(string id, Accommodation accommodation);

    Task DeleteAsync(string id);

}
