namespace StayFinder.Models;

// Model koji predstavlja jedan smeštaj (apartman, kuća, soba...)
public sealed class Accommodation
{
    public string? Id { get; set; }
    // Svaki smestaj pripada jednom Owner korisniku
    public string OwnerId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    // Cena po nocenju u dolarima
    public decimal PricePerNight { get; set; }
    // Lista usluga npr. "WiFi", "Parking", "Swimming pool"...
    public List<string> Amenities { get; set; } = new();
    // URL-ovi slika — prva u listi je uvek primarna (cover) slika
    public List<string> ImageUrls { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}
