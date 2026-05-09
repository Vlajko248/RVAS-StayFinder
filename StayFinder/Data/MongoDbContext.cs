using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StayFinder.Models;

namespace StayFinder.Data;

public sealed class MongoDbContext
{
    public MongoDbContext(IOptions<MongoDbSettings> options)
    {
        var settings = options.Value;
        var client = new MongoClient(settings.ConnectionString);
        Database = client.GetDatabase(settings.DatabaseName);

        Users = Database.GetCollection<User>(settings.UsersCollectionName);
        Accommodations = Database.GetCollection<Accommodation>(settings.AccommodationsCollectionName);
        Reservations = Database.GetCollection<Reservation>(settings.ReservationsCollectionName);
        Reviews = Database.GetCollection<Review>(settings.ReviewsCollectionName);
    }

    public IMongoDatabase Database { get; }
    public IMongoCollection<User> Users { get; }
    public IMongoCollection<Accommodation> Accommodations { get; }
    public IMongoCollection<Reservation> Reservations { get; }
    public IMongoCollection<Review> Reviews { get; }
}
