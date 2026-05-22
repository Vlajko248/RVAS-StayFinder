using MongoDB.Driver;
using StayFinder.Data;
using StayFinder.Models;

namespace StayFinder.Services;

public sealed class ReviewService : IReviewService
{
    private readonly IMongoCollection<Review> _reviews;
    private readonly IReservationService _reservationService;

    public ReviewService(MongoDbContext context, IReservationService reservationService)
    {
        _reviews = context.Reviews;
        _reservationService = reservationService;
    }

    public async Task<List<Review>> GetByAccommodationIdAsync(string accommodationId)
    {
        return await _reviews
            .Find(r => r.AccommodationId == accommodationId)
            .SortByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Review>> GetByGuestIdAsync(string guestId)
    {
        return await _reviews
            .Find(r => r.GuestId == guestId)
            .SortByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    // Korisnik ne moze vise puta da oceni isti smestaj.
    public async Task<bool> HasGuestReviewedAccommodationAsync(string guestId, string accommodationId)
    {
        return await _reviews
            .Find(r => r.GuestId == guestId && r.AccommodationId == accommodationId)
            .AnyAsync();
    }

    // Review je dozvoljen tek nakon zavrsene rezervacije.
    public async Task<bool> CanGuestReviewAccommodationAsync(string guestId, string accommodationId)
    {
        return await _reservationService.HasCompletedReservationAsync(guestId, accommodationId);
    }

    public async Task CreateAsync(Review review)
    {
        if (review.Rating is < 1 or > 5)
            throw new InvalidOperationException("Rating must be between 1 and 5.");

        if (string.IsNullOrWhiteSpace(review.AccommodationId))
            throw new InvalidOperationException("Accommodation is required.");

        if (string.IsNullOrWhiteSpace(review.GuestId))
            throw new InvalidOperationException("Guest is required.");

        if (await HasGuestReviewedAccommodationAsync(review.GuestId, review.AccommodationId))
            throw new InvalidOperationException("Guest already reviewed this accommodation.");

        // if (!await CanGuestReviewAccommodationAsync(review.GuestId, review.AccommodationId))
        //     throw new InvalidOperationException("Guest can review only after completed stay.");

        review.Id ??= Guid.NewGuid().ToString();
        review.Comment = review.Comment?.Trim() ?? string.Empty;
        review.CreatedAt = DateTime.UtcNow;

        await _reviews.InsertOneAsync(review);
    }
}
