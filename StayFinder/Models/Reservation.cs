namespace StayFinder.Models;

// Model koji predstavlja rezervaciju — gost rezervise smestaj za odredjeni period
public sealed class Reservation
{
    public string? Id { get; set; }
    // Referenca na smestaj koji se rezervise
    public string AccommodationId { get; set; } = string.Empty;
    // Referenca na gosta koji je napravio rezervaciju
    public string GuestId { get; set; } = string.Empty;
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    // Ukupna cena = broj nocenja * cena po nocenju, racuna se pri kreiranju
    public decimal TotalPrice { get; set; }
    // Status moze biti: "Active" (aktivna) ili "Completed" (zavrsena)
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
