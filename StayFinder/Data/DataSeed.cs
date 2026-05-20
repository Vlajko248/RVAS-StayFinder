using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using StayFinder.Models;


/// ovo je testni data seed koji ce ubaciti testne podatke u bazu

namespace StayFinder.Data
{
    public class DataSeed
    {
        public static async Task SeedAsync(MongoDbContext context)
        {
            // 1. Provera da li vec ima podataka
            if (await context.Users.CountDocumentsAsync(_ => true) > 0)
                return;

            var passwordHasher = new PasswordHasher<User>();

            // 2. USERS
            var owner = new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = "owner@test.com",
                FullName = "Owner User",
                Role = "Owner",
                CreatedAt = DateTime.UtcNow
            };
            owner.PasswordHash = passwordHasher.HashPassword(owner, "test123");

            var guest = new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = "guest@test.com",
                FullName = "Guest User",
                Role = "Guest",
                CreatedAt = DateTime.UtcNow
            };
            guest.PasswordHash = passwordHasher.HashPassword(guest, "test123");

            await context.Users.InsertManyAsync(new[] { owner, guest });

            // 3. ACCOMMODATIONS
            var acc1 = new Accommodation
            {
                Id = Guid.NewGuid().ToString(),
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
                Id = Guid.NewGuid().ToString(),
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

            // 4. RESERVATIONS
            var activeReservation = new Reservation
            {
                Id = Guid.NewGuid().ToString(),
                AccommodationId = acc1.Id!,
                GuestId = guest.Id!,
                DateFrom = DateTime.UtcNow.AddDays(1),
                DateTo = DateTime.UtcNow.AddDays(3),
                TotalPrice = 100,
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            };

            var completedReservation = new Reservation
            {
                Id = Guid.NewGuid().ToString(),
                AccommodationId = acc2.Id!,
                GuestId = guest.Id!,
                DateFrom = DateTime.UtcNow.AddDays(-10),
                DateTo = DateTime.UtcNow.AddDays(-7),
                TotalPrice = 210,
                Status = "Completed",
                CreatedAt = DateTime.UtcNow.AddDays(-11)
            };

            await context.Reservations.InsertManyAsync(new[] { activeReservation, completedReservation });

            // 5. REVIEW
            var review = new Review
            {
                Id = Guid.NewGuid().ToString(),
                AccommodationId = acc2.Id!,
                GuestId = guest.Id!,
                Rating = 5,
                Comment = "Odlican smestaj, sve preporuke.",
                CreatedAt = DateTime.UtcNow.AddDays(-6)
            };

            await context.Reviews.InsertOneAsync(review);
        }
    }
}
