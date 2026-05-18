using MongoDB.Driver;
using StayFinder.Data;
using StayFinder.Models;

namespace StayFinder.Services;

public sealed class ReservationService : IReservationService
{
    // TODO: Implement reservation business logic.
    ///nastavak
    ///
    private readonly IMongoCollection<Reservation> _reservations;

    public ReservationService(MongoDbContext context)
    {
        _reservations = context.Reservations;
    }

    public async Task<List<Reservation>> GetAllAsync()
    {
        return await _reservations
            .Find(_ => true)
            .ToListAsync();
    }

    public async Task<List<Reservation>> GetByGuestIdAsync(string guestId)
    {
        return await _reservations
            .Find(r => r.GuestId == guestId)
            .ToListAsync();
    }

    public async Task<List<Reservation>> GetByAccommodationIdAsync(string accommodationId)
    {
        return await _reservations
            .Find(r => r.AccommodationId == accommodationId)
            .ToListAsync();
    }

    public async Task<bool> IsAccommodationAvailableAsync(
        string accommodationId,
        DateTime dateFrom,
        DateTime dateTo)
    {
        var overlappingReservation = await _reservations
            .Find(r =>
                r.AccommodationId == accommodationId &&
                r.Status == "Active" &&
                r.DateFrom < dateTo &&
                dateFrom < r.DateTo)
            .FirstOrDefaultAsync();

        return overlappingReservation == null;
    }

    public async Task CreateAsync(Reservation reservation)
    {
        reservation.Id ??= Guid.NewGuid().ToString();
        reservation.CreatedAt = DateTime.UtcNow;
        reservation.Status ??= "Active";

        await _reservations.InsertOneAsync(reservation);
    }

    public async Task DeleteAsync(string id)
    {
        await _reservations.DeleteOneAsync(r => r.Id == id);
    }

}
