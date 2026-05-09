# StayFinder

ASP.NET Core MVC aplikacija za izdavanje smeštaja, sa MongoDB bazom.

## Pokretanje

1. Instalirati .NET 9 SDK.
2. Pokrenuti MongoDB lokalno ili podesiti svoju konekciju. (https://fastdl.mongodb.org/windows/mongodb-windows-x86_64-8.3.1-signed.msi)
3. Proveriti `appsettings.json` i sekciju `MongoDbSettings`.
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

## Napomena

Projektna osnova je spremna za dalji timski rad. CRUD logika, validacija i autorizacija treba da se implementiraju naknadno.
