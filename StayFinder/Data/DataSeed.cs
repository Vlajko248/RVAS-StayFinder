using StayFinder.Models;
using MongoDB.Driver;


/// ovo je testni data seed koji ce ubaciti testne podatke u bazu
 
namespace StayFinder.Data
{
    public class DataSeed
    {
        public static async Task SeedAsync(MongoDbContext context)
        {
            // 1. Provera da li već ima podataka
            if (await context.Users.CountDocumentsAsync(_ => true) > 0)
                return;

            // 2. USERS
            var owner = new User
            {
                Email = "owner@test.com",
                FullName = "Owner User",
                Role = "Owner",
                PasswordHash = "test",
                CreatedAt = DateTime.UtcNow
            };

            var guest = new User
            {
                Email = "guest@test.com",
                FullName = "Guest User",
                Role = "Guest",
                PasswordHash = "test",
                CreatedAt = DateTime.UtcNow
            };

            await context.Users.InsertManyAsync(new[] { owner, guest });

            // 3. ACCOMMODATIONS
            var acc1 = new Accommodation
            {
                OwnerId = owner.Id!,
                Name = "Apartman 1",
                Location = "Novi Sad",
                Description = "Lep apartman",
                PricePerNight = 50,
                Amenities = new List<string> { "WiFi", "Klima" },
                ImageUrls = new List<string>(),
                CreatedAt = DateTime.UtcNow
            };

            var acc2 = new Accommodation
            {
                OwnerId = owner.Id!,
                Name = "Apartman 2",
                Location = "Beograd",
                Description = "Centar grada",
                PricePerNight = 70,
                Amenities = new List<string> { "WiFi", "Parking" },
                ImageUrls = new List<string>(),
                CreatedAt = DateTime.UtcNow
            };

            await context.Accommodations.InsertManyAsync(new[] { acc1, acc2 });

            // 4. RESERVATION
            var reservation = new Reservation
            {
                AccommodationId = acc1.Id!,
                GuestId = guest.Id!,
                DateFrom = DateTime.UtcNow.AddDays(1),
                DateTo = DateTime.UtcNow.AddDays(3),
                TotalPrice = 100,
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            };

            await context.Reservations.InsertOneAsync(reservation);
        }
    }
}
