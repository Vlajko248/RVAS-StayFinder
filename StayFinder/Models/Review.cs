namespace StayFinder.Models;

public sealed class Review
{
    public string? Id { get; set; }
    public string AccommodationId { get; set; } = string.Empty;
    public string GuestId { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
