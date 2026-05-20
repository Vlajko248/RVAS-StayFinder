namespace StayFinder.Services;

public interface IFileUploadService
{
    bool IsValidImage(IFormFile file, out string? validationError);
    Task<string> UploadAccommodationImageAsync(IFormFile file, CancellationToken cancellationToken = default);
}
