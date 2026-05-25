namespace StayFinder.Data;

// Ova klasa mapira sekciju "MongoDbSettings" iz appsettings.json
// Sve vrednosti se ucitavaju automatski kroz IOptions<MongoDbSettings> u DI
public sealed class MongoDbSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    // Nazivi kolekcija u MongoDB bazi
    public string UsersCollectionName { get; set; } = string.Empty;
    public string AccommodationsCollectionName { get; set; } = string.Empty;
    public string ReservationsCollectionName { get; set; } = string.Empty;
    public string ReviewsCollectionName { get; set; } = string.Empty;
}
