using StayFinder.Models;

namespace StayFinder.Services;


public interface IReservationService
{
    // TODO: Add reservation management operations.
    ///nastavak
    Task<List<Reservation>> GetAllAsync();
    Task<Reservation?> GetByIdAsync(string id);
    Task<List<Reservation>> GetByGuestIdAsync(string guestId);
    Task<List<Reservation>> GetByAccommodationIdAsync(string accommodationId);

    Task<bool> IsAccommodationAvailableAsync(
        string accommodationId,
        DateTime dateFrom,
        DateTime dateTo);

    Task<bool> HasCompletedReservationAsync(string guestId, string accommodationId);

    Task CreateAsync(Reservation reservation);

    Task DeleteAsync(string id);
}
