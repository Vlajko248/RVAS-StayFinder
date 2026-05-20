# StayFinder

ASP.NET Core MVC aplikacija za izdavanje smeštaja, sa MongoDB bazom.

## Pokretanje

1. Instalirati .NET 9 SDK.
2. Pokrenuti MongoDB lokalno ili podesiti svoju konekciju. (https://fastdl.mongodb.org/windows/mongodb-windows-x86_64-8.3.1-signed.msi)
3. Proveriti `appsettings.json` i sekciju `MongoDbSettings`. Baza: "StayFinderDb"
4. Pokrenuti projekat:

```bash
dotnet restore
dotnet run
```

## Struktura

- `Controllers` - MVC kontroleri
- `Models` - domen modeli
- `ViewModels` - pomoćni modeli za prikaz
- `Services` - servisni sloj
- `Data` - MongoDB podešavanja i kontekst
- `wwwroot/uploads` - upload folder za slike smeštaja

## Minimalno bezbedan custom auth (uradjeno)

- lozinke se hešuju preko `PasswordHasher`
- session je ukljucen u `Program.cs` (`AddSession` + `UseSession`)
- email se normalizuje pri registraciji/loginu
- registracija ima osnovnu validaciju
- novi korisnici dobijaju rolu `Guest`

## Backend dorade (uradjeno)

- svi kontroleri su uskladjeni kao MVC kontroleri
- owner/guest tokovi koriste centralizovane session helper-e
- owner može da radi CRUD samo nad svojim smeštajima
- rezervacije su vezane za trenutno ulogovanog gosta
- owner može da vidi rezervacije samo svojih smeštaja
- upload slika radi u `wwwroot/uploads` uz validaciju tipa i veličine
- review modul je dodat (`ReviewService` + `ReviewController`)
- review je dozvoljen samo gostu koji je imao završen boravak

## Test korisnici (DataSeed)

Ako je baza prazna, seed ubacuje:

- `owner@test.com` / `test123`
- `guest@test.com` / `test123`

Seed ubacuje i:

- 2 smeštaja
- 1 aktivnu rezervaciju
- 1 završenu rezervaciju
- 1 primer recenzije
