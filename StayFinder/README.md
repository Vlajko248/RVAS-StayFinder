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

Nismo uvodili full Identity, nego smo uradili minimum da auth ne bude "plain text" i da može normalno za studentski projekat:

- lozinke se hešuju preko `PasswordHasher` (vise se ne cuva plain text)
- session je ukljucen u `Program.cs` (`AddSession` + `UseSession`)
- session cookie je `HttpOnly` i sa osnovnim bezbednosnim opcijama
- email se normalizuje (trim + lower) pri registraciji i loginu
- registracija ima osnovnu validaciju (`email`, min duzina lozinke)
- novi korisnici dobijaju rolu `Guest` (da ne moze role escalation preko body-ja)
- `Profile` endpoint ne vraca `PasswordHash`
- stari test korisnici iz seeda su prebaceni na hešovane lozinke

## Test korisnici (DataSeed)

Ako je baza prazna, seed ubacuje:

- `owner@test.com` / `test123`
- `guest@test.com` / `test123`

## Napomena

Projektna osnova je spremna za dalji timski rad. CRUD logika, validacija i autorizacija treba da se implementiraju naknadno.
