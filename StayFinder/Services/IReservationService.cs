using StayFinder.Models;

namespace StayFinder.Services;


// Interfejs za servis rezervacija
public interface IReservationService
{
    Task<List<Reservation>> GetAllAsync();
    Task<Reservation?> GetByIdAsync(string id);
    // Sve rezervacije jednog gosta — koristi se na MyReservations strani
    Task<List<Reservation>> GetByGuestIdAsync(string guestId);
    // Sve rezervacije jednog smestaja — koristi se za prikaz zauzetih termina
    Task<List<Reservation>> GetByAccommodationIdAsync(string accommodationId);
    // Proverava da li se trazeni period preklapa sa vec postojecim aktivnim rezervacijama
    Task<bool> IsAccommodationAvailableAsync(
        string accommodationId,
        DateTime dateFrom,
        DateTime dateTo);
    Task CreateAsync(Reservation reservation);
    Task DeleteAsync(string id);
}
