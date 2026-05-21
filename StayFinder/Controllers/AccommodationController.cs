using Microsoft.AspNetCore.Mvc;
using StayFinder.Extensions;
using StayFinder.Models;
using StayFinder.Services;

namespace StayFinder.Controllers;

public class AccommodationController : Controller
{
    private readonly IAccommodationService _accommodationService;
    private readonly IFileUploadService _fileUploadService;
    private readonly IReservationService _reservationService;

    public AccommodationController(
        IAccommodationService accommodationService,
        IFileUploadService fileUploadService,
        IReservationService reservationService)
    {
        _accommodationService = accommodationService;
        _fileUploadService = fileUploadService;
        _reservationService = reservationService;
    }

    // Javni listing svih smestaja + opcioni filter po lokaciji i datumima.
    [HttpGet]
    public async Task<IActionResult> Index(string? location, string? start, string? end)
    {
        var accommodations = await _accommodationService.SearchByLocationAsync(location);

        if (!string.IsNullOrWhiteSpace(start) &&
            !string.IsNullOrWhiteSpace(end) &&
            DateTime.TryParse(start, out var startDate) &&
            DateTime.TryParse(end, out var endDate) &&
            startDate < endDate)
        {
            var availableAccommodations = new List<Accommodation>();

            foreach (var accommodation in accommodations)
            {
                if (string.IsNullOrWhiteSpace(accommodation.Id))
                    continue;

                var isAvailable = await _reservationService.IsAccommodationAvailableAsync(
                    accommodation.Id,
                    startDate,
                    endDate);

                if (isAvailable)
                    availableAccommodations.Add(accommodation);
            }

            accommodations = availableAccommodations;
        }

        return View(accommodations);
    }

    // Detalji jednog smestaja.
    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
        var accommodation = await _accommodationService.GetByIdAsync(id);

        if (accommodation == null)
            return NotFound();

        return View(accommodation);
    }

    // Owner vidi samo svoje smestaje.
    [HttpGet]
    public async Task<IActionResult> MyAccommodations()
    {
        var ownerId = HttpContext.GetCurrentUserId();
        if (!HttpContext.IsInRole("Owner") || string.IsNullOrWhiteSpace(ownerId))
            return RedirectToAction("Login", "Account");

        var accommodations = await _accommodationService.GetByOwnerIdAsync(ownerId);
        return View(accommodations);
    }

    [HttpGet]
    public IActionResult Create()
    {
        if (!HttpContext.IsInRole("Owner"))
            return RedirectToAction("Login", "Account");

        return View();
    }

    // Kreiranje smestaja + opcioni upload jedne slike.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Accommodation accommodation, IFormFile? imageFile)
    {
        var ownerId = HttpContext.GetCurrentUserId();
        if (!HttpContext.IsInRole("Owner") || string.IsNullOrWhiteSpace(ownerId))
            return RedirectToAction("Login", "Account");

        try
        {
            accommodation.OwnerId = ownerId;
            accommodation.ImageUrls ??= new List<string>();

            if (imageFile is not null)
            {
                var imageUrl = await _fileUploadService.UploadAccommodationImageAsync(imageFile);
                accommodation.ImageUrls.Add(imageUrl);
            }

            await _accommodationService.CreateAsync(accommodation);
            return RedirectToAction(nameof(MyAccommodations));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(accommodation);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var ownerId = HttpContext.GetCurrentUserId();
        if (!HttpContext.IsInRole("Owner") || string.IsNullOrWhiteSpace(ownerId))
            return RedirectToAction("Login", "Account");

        var accommodation = await _accommodationService.GetByIdAsync(id);
        if (accommodation == null)
            return NotFound();

        if (accommodation.OwnerId != ownerId)
            return Forbid();

        return View(accommodation);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, Accommodation accommodation, IFormFile? imageFile)
    {
        var ownerId = HttpContext.GetCurrentUserId();
        if (!HttpContext.IsInRole("Owner") || string.IsNullOrWhiteSpace(ownerId))
            return RedirectToAction("Login", "Account");

        var existingAccommodation = await _accommodationService.GetByIdAsync(id);
        if (existingAccommodation == null)
            return NotFound();

        if (existingAccommodation.OwnerId != ownerId)
            return Forbid();

        try
        {
            accommodation.OwnerId = ownerId;
            accommodation.CreatedAt = existingAccommodation.CreatedAt;
            accommodation.ImageUrls = existingAccommodation.ImageUrls ?? new List<string>();

            if (imageFile is not null)
            {
                var imageUrl = await _fileUploadService.UploadAccommodationImageAsync(imageFile);
                accommodation.ImageUrls.Add(imageUrl);
            }

            await _accommodationService.UpdateAsync(id, accommodation);
            return RedirectToAction(nameof(MyAccommodations));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(accommodation);
        }
    }

    // Brisanje dozvoljeno samo vlasniku konkretnog smestaja.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var ownerId = HttpContext.GetCurrentUserId();
        if (!HttpContext.IsInRole("Owner") || string.IsNullOrWhiteSpace(ownerId))
            return RedirectToAction("Login", "Account");

        var isOwner = await _accommodationService.IsOwnerOfAccommodationAsync(id, ownerId);
        if (!isOwner)
            return Forbid();

        await _accommodationService.DeleteAsync(id);
        return RedirectToAction(nameof(MyAccommodations));
    }
}
