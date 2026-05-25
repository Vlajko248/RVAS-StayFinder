using MongoDB.Bson;
using MongoDB.Driver;
using StayFinder.Data;
using StayFinder.Models;

namespace StayFinder.Services;

public sealed class AccommodationService : IAccommodationService
{
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

    public async Task<List<Accommodation>> SearchByLocationAsync(string? location)
    {
        if (string.IsNullOrWhiteSpace(location))
            return await GetAllAsync();

        var normalizedLocation = location.Trim();
        var filter = Builders<Accommodation>.Filter.Regex(
            a => a.Location,
            new BsonRegularExpression(normalizedLocation, "i"));

        return await _accommodations
            .Find(filter)
            .ToListAsync();
    }

    public async Task<List<Accommodation>> GetByOwnerIdAsync(string ownerId)
    {
        return await _accommodations
            .Find(a => a.OwnerId == ownerId)
            .ToListAsync();
    }

    public async Task<Accommodation?> GetByIdAsync(string id)
    {
        return await _accommodations
            .Find(a => a.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> IsOwnerOfAccommodationAsync(string accommodationId, string ownerId)
    {
        return await _accommodations
            .Find(a => a.Id == accommodationId && a.OwnerId == ownerId)
            .AnyAsync();
    }

    public async Task CreateAsync(Accommodation accommodation)
    {
        // Validacija pre upisa — baca InvalidOperationException ako nesto ne valja
        ValidateAccommodation(accommodation);

        // Ako ID nije prosledjen, generisemo novi GUID
        accommodation.Id ??= Guid.NewGuid().ToString();
        accommodation.CreatedAt = DateTime.UtcNow;

        await _accommodations.InsertOneAsync(accommodation);
    }

    public async Task UpdateAsync(string id, Accommodation accommodation)
    {
        ValidateAccommodation(accommodation);
        // Osiguravamo da ID ostaje isti kao u ruti — ne sme da se promeni
        accommodation.Id = id;

        // ReplaceOne menja ceo dokument sa istim _id
        await _accommodations.ReplaceOneAsync(a => a.Id == id, accommodation);
    }

    public async Task DeleteAsync(string id)
    {
        await _accommodations.DeleteOneAsync(a => a.Id == id);
    }

    private static void ValidateAccommodation(Accommodation accommodation)
    {
        if (string.IsNullOrWhiteSpace(accommodation.Name))
            throw new InvalidOperationException("Accommodation name is required.");

        if (string.IsNullOrWhiteSpace(accommodation.Location))
            throw new InvalidOperationException("Accommodation location is required.");

        if (accommodation.PricePerNight <= 0)
            throw new InvalidOperationException("Price per night must be greater than 0.");
    }
}
