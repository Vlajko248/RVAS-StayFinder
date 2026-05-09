namespace StayFinder.Models;

public sealed class Reservation
{
    public string? Id { get; set; }
    public string AccommodationId { get; set; } = string.Empty;
    public string GuestId { get; set; } = string.Empty;
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
