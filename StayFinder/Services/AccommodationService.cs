using MongoDB.Driver;
using StayFinder.Data;
using StayFinder.Models;

namespace StayFinder.Services;

public sealed class AccommodationService : IAccommodationService
{
    // TODO: Implement accommodation business logic.
    //ok nastavak za backend "Stevan"
    private readonly IMongoCollection<Accommodation> _accommodations;

    public AccommodationService(MongoDbContext context)
    {
        _accommodations = context.Accommodations;
    }

    public async Task<List<Accommodation>> GetAllAsync()
    {
        return await _accommodations
            .Find(_ => true)
            .ToListAsync();
    }

    public async Task<Accommodation?> GetByIdAsync(string id)
    {
        return await _accommodations
            .Find(a => a.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Accommodation accommodation)
    {
        ///zastita ukoliko se na fronted ne postavi id
        accommodation.Id ??= Guid.NewGuid().ToString();
        accommodation.CreatedAt = DateTime.UtcNow; 

        await _accommodations.InsertOneAsync(accommodation);
    }

    public async Task UpdateAsync(string id, Accommodation accommodation)
    {
        await _accommodations.ReplaceOneAsync(a => a.Id == id, accommodation);
    }

    public async Task DeleteAsync(string id)
    {
        await _accommodations.DeleteOneAsync(a => a.Id == id);
    }

}
