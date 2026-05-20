using StayFinder.Models;

namespace StayFinder.Services;

public interface IReviewService
{
    Task<List<Review>> GetByAccommodationIdAsync(string accommodationId);
    Task<List<Review>> GetByGuestIdAsync(string guestId);
    Task<bool> HasGuestReviewedAccommodationAsync(string guestId, string accommodationId);
    Task<bool> CanGuestReviewAccommodationAsync(string guestId, string accommodationId);
    Task CreateAsync(Review review);
}
