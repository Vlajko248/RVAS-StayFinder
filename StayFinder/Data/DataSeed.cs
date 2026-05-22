using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using StayFinder.Models;

/// Bogatiji seed — vise korisnika, smestaja, rezervacija i reviewova za demo/odbranu

namespace StayFinder.Data
{
    public class DataSeed
    {
        public static async Task SeedAsync(MongoDbContext context)
        {
            // Ako vec ima podataka, preskacemo seed
            if (await context.Users.CountDocumentsAsync(_ => true) > 0)
                return;

            var hasher = new PasswordHasher<User>();

            // ================================
            // KORISNICI
            // ================================

            var owner1 = MakeUser(hasher, "marko.petrovic@test.com", "Marko Petrović", "Owner");
            var owner2 = MakeUser(hasher, "ana.jovic@test.com", "Ana Jović", "Owner");

            var guest1 = MakeUser(hasher, "nikola.nikolic@test.com", "Nikola Nikolić", "Guest");
            var guest2 = MakeUser(hasher, "jovana.stankovic@test.com", "Jovana Stanković", "Guest");
            var guest3 = MakeUser(hasher, "stefan.ilic@test.com", "Stefan Ilić", "Guest");

            await context.Users.InsertManyAsync(new[] { owner1, owner2, guest1, guest2, guest3 });

            // ================================
            // SMESTAJI
            // ================================

            var accommodations = new List<Accommodation>
            {
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    OwnerId = owner1.Id!,
                    Name = "Lux apartman u centru",
                    Location = "Beograd",
                    Description = "Moderan apartman u samom centru Beograda, 5 min od Knez Mihailove. Pogled na reku, parking u dvorištu.",
                    PricePerNight = 120,
                    Amenities = new List<string> { "WiFi", "Air Condition", "Parking", "Kitchen", "TV" },
                    ImageUrls = new List<string> { "/img/hotels/d1.jpg" },
                    CreatedAt = DateTime.UtcNow.AddDays(-60)
                },
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    OwnerId = owner1.Id!,
                    Name = "Studio na Voždovcu",
                    Location = "Beograd",
                    Description = "Kompaktan studio apartman, idealan za poslovne putnike. Blizina BAS-a i Autokomande.",
                    PricePerNight = 55,
                    Amenities = new List<string> { "WiFi", "Air Condition", "TV" },
                    ImageUrls = new List<string> { "/img/hotels/d1.jpg" },
                    CreatedAt = DateTime.UtcNow.AddDays(-50)
                },
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    OwnerId = owner1.Id!,
                    Name = "Kuća sa bazenom — Zlatibor",
                    Location = "Zlatibor",
                    Description = "Planinska kuća sa privatnim bazenom, roštiljem i panoramskim pogledom na šumu. Idealno za porodice.",
                    PricePerNight = 200,
                    Amenities = new List<string> { "WiFi", "Swimming pool", "Parking", "Kitchen", "TV", "Gym" },
                    ImageUrls = new List<string> { "/img/hotels/d1.jpg" },
                    CreatedAt = DateTime.UtcNow.AddDays(-45)
                },
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    OwnerId = owner2.Id!,
                    Name = "Apartman na Petrovaradinu",
                    Location = "Novi Sad",
                    Description = "Renoviran apartman sa pogledom na Tvrđavu. 2 sobe, parking, brzi WiFi.",
                    PricePerNight = 80,
                    Amenities = new List<string> { "WiFi", "Parking", "Air Condition", "Kitchen" },
                    ImageUrls = new List<string> { "/img/hotels/d1.jpg" },
                    CreatedAt = DateTime.UtcNow.AddDays(-40)
                },
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    OwnerId = owner2.Id!,
                    Name = "Soba u centru Novog Sada",
                    Location = "Novi Sad",
                    Description = "Prijatna soba sa zasebnim kupatilom, tik uz Zmaj Jovina ulica. Doručak uključen.",
                    PricePerNight = 35,
                    Amenities = new List<string> { "WiFi", "TV" },
                    ImageUrls = new List<string> { "/img/hotels/d1.jpg" },
                    CreatedAt = DateTime.UtcNow.AddDays(-35)
                },
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    OwnerId = owner2.Id!,
                    Name = "Vikendica na Fruškoj Gori",
                    Location = "Fruška Gora",
                    Description = "Mirna vikendica u srcu Fruške Gore. Šetnja do vinograda, planinarenje, potpuna tišina.",
                    PricePerNight = 90,
                    Amenities = new List<string> { "Parking", "Kitchen", "Restaurant" },
                    ImageUrls = new List<string> { "/img/hotels/d1.jpg" },
                    CreatedAt = DateTime.UtcNow.AddDays(-30)
                },
            };

            await context.Accommodations.InsertManyAsync(accommodations);

            var acc = accommodations; // alias za citljivost

            // ================================
            // REZERVACIJE
            // ================================

            var reservations = new List<Reservation>
            {
                // Aktivne — buduce
                MakeReservation(acc[0].Id!, guest1.Id!, DateTime.UtcNow.AddDays(3),  DateTime.UtcNow.AddDays(7),   acc[0].PricePerNight, "Active"),
                MakeReservation(acc[2].Id!, guest2.Id!, DateTime.UtcNow.AddDays(10), DateTime.UtcNow.AddDays(15),  acc[2].PricePerNight, "Active"),
                MakeReservation(acc[3].Id!, guest3.Id!, DateTime.UtcNow.AddDays(1),  DateTime.UtcNow.AddDays(4),   acc[3].PricePerNight, "Active"),
                MakeReservation(acc[5].Id!, guest1.Id!, DateTime.UtcNow.AddDays(20), DateTime.UtcNow.AddDays(23),  acc[5].PricePerNight, "Active"),

                // Zavrsene — prosle
                MakeReservation(acc[0].Id!, guest2.Id!, DateTime.UtcNow.AddDays(-30), DateTime.UtcNow.AddDays(-26), acc[0].PricePerNight, "Completed", DateTime.UtcNow.AddDays(-32)),
                MakeReservation(acc[1].Id!, guest1.Id!, DateTime.UtcNow.AddDays(-20), DateTime.UtcNow.AddDays(-18), acc[1].PricePerNight, "Completed", DateTime.UtcNow.AddDays(-22)),
                MakeReservation(acc[3].Id!, guest2.Id!, DateTime.UtcNow.AddDays(-15), DateTime.UtcNow.AddDays(-12), acc[3].PricePerNight, "Completed", DateTime.UtcNow.AddDays(-17)),
                MakeReservation(acc[4].Id!, guest3.Id!, DateTime.UtcNow.AddDays(-8),  DateTime.UtcNow.AddDays(-6),  acc[4].PricePerNight, "Completed", DateTime.UtcNow.AddDays(-10)),
            };

            await context.Reservations.InsertManyAsync(reservations);

            // ================================
            // REVIEWOVI
            // ================================

            var reviews = new List<Review>
            {
                MakeReview(acc[0].Id!, guest2.Id!, 5, "Savršen apartman, sve na svom mestu. Definitivno se vraćamo!", DateTime.UtcNow.AddDays(-25)),
                MakeReview(acc[1].Id!, guest1.Id!, 4, "Lepo i čisto, malo manja soba nego na slikama ali sve u svemu odlično.", DateTime.UtcNow.AddDays(-17)),
                MakeReview(acc[3].Id!, guest2.Id!, 5, "Pogled na Tvrđavu je fenomenalan. Domaćin super ljubazan.", DateTime.UtcNow.AddDays(-11)),
                MakeReview(acc[4].Id!, guest3.Id!, 3, "Dobra lokacija, ali buka sa ulice je problem. Doručak je bio odličan.", DateTime.UtcNow.AddDays(-5)),
            };

            await context.Reviews.InsertManyAsync(reviews);
        }

        // ================================
        // HELPER METODE
        // ================================

        private static User MakeUser(PasswordHasher<User> hasher, string email, string fullName, string role)
        {
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = email,
                FullName = fullName,
                Role = role,
                CreatedAt = DateTime.UtcNow.AddDays(-70)
            };
            user.PasswordHash = hasher.HashPassword(user, "test123");
            return user;
        }

        private static Reservation MakeReservation(string accId, string guestId, DateTime from, DateTime to, decimal pricePerNight, string status, DateTime? createdAt = null)
        {
            var nights = Math.Max(1, (to.Date - from.Date).Days);
            return new Reservation
            {
                Id = Guid.NewGuid().ToString(),
                AccommodationId = accId,
                GuestId = guestId,
                DateFrom = from,
                DateTo = to,
                TotalPrice = nights * pricePerNight,
                Status = status,
                CreatedAt = createdAt ?? DateTime.UtcNow.AddDays(-2)
            };
        }

        private static Review MakeReview(string accId, string guestId, int rating, string comment, DateTime createdAt)
        {
            return new Review
            {
                Id = Guid.NewGuid().ToString(),
                AccommodationId = accId,
                GuestId = guestId,
                Rating = rating,
                Comment = comment,
                CreatedAt = createdAt
            };
        }
    }
}
