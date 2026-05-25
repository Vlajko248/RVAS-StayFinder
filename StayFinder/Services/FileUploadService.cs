namespace StayFinder.Services;

public sealed class FileUploadService : IFileUploadService
{
    // Jednostavna ogranicenja za studentski projekat.
    private const long MaxFileSizeBytes = 5 * 1024 * 1024;
    private static readonly HashSet<string> AllowedExtensions =
    [
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    ];

    private readonly IWebHostEnvironment _environment;

    public FileUploadService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    // Provera da li je fajl validna slika pre upisa na disk.
    public bool IsValidImage(IFormFile file, out string? validationError)
    {
        validationError = null;

        if (file == null || file.Length == 0)
        {
            validationError = "Image file is required.";
            return false;
        }

        if (file.Length > MaxFileSizeBytes)
        {
            validationError = "Image must be up to 5MB.";
            return false;
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
        {
            validationError = "Only .jpg, .jpeg, .png and .webp images are allowed.";
            return false;
        }

        return true;
    }

    // Snima fajl u wwwroot/uploads i vraca relativnu putanju za prikaz u <img src="...">
    public async Task<string> UploadAccommodationImageAsync(IFormFile file)
    {
        if (!IsValidImage(file, out var validationError))
            throw new InvalidOperationException(validationError);

        var webRootPath = _environment.WebRootPath;
        if (string.IsNullOrWhiteSpace(webRootPath))
            throw new InvalidOperationException("Web root path is not configured.");

        // Kreiranje foldera ako ne postoji
        var uploadsDirectory = Path.Combine(webRootPath, "uploads");
        Directory.CreateDirectory(uploadsDirectory);

        // Ime fajla je GUID da izbegnemo konflikte i da ne izlazimo plain korisnicka imena
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var fileName = $"{Guid.NewGuid():N}{extension}";
        var filePath = Path.Combine(uploadsDirectory, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        // Vracamo web-relativnu putanju koja se direktno koristi kao src u HTML-u
        return $"/uploads/{fileName}";
    }
}
