using MongoDB.Driver;
using StayFinder.Data;
using StayFinder.Models;

namespace StayFinder.Services;

public sealed class ReservationService : IReservationService
{
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

    public async Task<Reservation?> GetByIdAsync(string id)
    {
        return await _reservations
            .Find(r => r.Id == id)
            .FirstOrDefaultAsync();
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

    public async Task<bool> IsAccommodationAvailableAsync(string accommodationId, DateTime dateFrom, DateTime dateTo)
    {
        // Klasicna provera preklapanja intervala: A < B_end && B_start < A_end
        // Ako postoji bar jedna aktivna rezervacija koja se preklapa — smestaj nije slobodan
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
        ValidateReservation(reservation);

        reservation.Id ??= Guid.NewGuid().ToString();
        reservation.CreatedAt = DateTime.UtcNow;
        reservation.Status = string.IsNullOrWhiteSpace(reservation.Status) ? "Active" : reservation.Status;

        await _reservations.InsertOneAsync(reservation);
    }

    public async Task DeleteAsync(string id)
    {
        await _reservations.DeleteOneAsync(r => r.Id == id);
    }

    private static void ValidateReservation(Reservation reservation)
    {
        if (string.IsNullOrWhiteSpace(reservation.AccommodationId))
            throw new InvalidOperationException("Accommodation is required.");

        if (string.IsNullOrWhiteSpace(reservation.GuestId))
            throw new InvalidOperationException("Guest is required.");

        if (reservation.DateFrom >= reservation.DateTo)
            throw new InvalidOperationException("Date range is invalid.");

        if (reservation.TotalPrice <= 0)
            throw new InvalidOperationException("Total price must be greater than 0.");
    }
}
