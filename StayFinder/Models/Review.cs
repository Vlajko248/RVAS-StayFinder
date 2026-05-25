namespace StayFinder.Models;

// Model koji predstavlja recenziju — gost ocenjuje smestaj nakon boravka
public sealed class Review
{
    public string? Id { get; set; }
    // Smestaj koji se ocenjuje
    public string AccommodationId { get; set; } = string.Empty;
    // Gost koji je ostavio recenziju
    public string GuestId { get; set; } = string.Empty;
    // Ocena od 1 do 5 — validacija je u ReviewService
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
