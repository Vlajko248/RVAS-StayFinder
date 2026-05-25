using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StayFinder.Models;

namespace StayFinder.Data;

// Centralno mesto za pristup MongoDB kolekcijama — registrovan kao Singleton u DI
public sealed class MongoDbContext
{
    // Konstruktor prima opcije iz appsettings.json i otvara konekciju ka bazi
    public MongoDbContext(IOptions<MongoDbSettings> options)
    {
        var settings = options.Value;
        var client = new MongoClient(settings.ConnectionString);
        Database = client.GetDatabase(settings.DatabaseName);

        // Svaka kolekcija odgovara jednoj MongoDB kolekciji po imenu iz settings-a
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
