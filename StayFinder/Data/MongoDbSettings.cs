namespace StayFinder.Data;

public sealed class MongoDbSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string UsersCollectionName { get; set; } = string.Empty;
    public string AccommodationsCollectionName { get; set; } = string.Empty;
    public string ReservationsCollectionName { get; set; } = string.Empty;
    public string ReviewsCollectionName { get; set; } = string.Empty;
}
