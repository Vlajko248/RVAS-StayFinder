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
    private readonly IReviewService _reviewService;
    private readonly IUserService _userService;

    public AccommodationController(
        IAccommodationService accommodationService,
        IFileUploadService fileUploadService,
        IReservationService reservationService,
        IReviewService reviewService,
        IUserService userService)
    {
        _accommodationService = accommodationService;
        _fileUploadService = fileUploadService;
        _reservationService = reservationService;
        _reviewService = reviewService;
        _userService = userService;
    }

    // Javni listing svih smestaja + opcioni filter po lokaciji, datumima i ceni.
    [HttpGet]
    public async Task<IActionResult> Index(string? location, string? start, string? end, decimal? minPrice, decimal? maxPrice)
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

        // Filter po ceni — ako je korisnik uneo min/max
        if (minPrice.HasValue)
            accommodations = accommodations.Where(a => a.PricePerNight >= minPrice.Value).ToList();

        if (maxPrice.HasValue)
            accommodations = accommodations.Where(a => a.PricePerNight <= maxPrice.Value).ToList();

        return View(accommodations);
    }

    // Detalji jednog smestaja — sadrzi i listu zauzetih termina i korisnike za prikaz imena.
    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
        var accommodation = await _accommodationService.GetByIdAsync(id);

        if (accommodation == null)
            return NotFound();

        var reviews = await _reviewService.GetByAccommodationIdAsync(id);
        ViewBag.Reviews = reviews;

        // Zauzeti termini za prikaz na stranici detalja
        var reservations = await _reservationService.GetByAccommodationIdAsync(id);
        ViewBag.BookedPeriods = reservations
            .Where(r => r.Status == "Active" && r.DateTo >= DateTime.UtcNow)
            .OrderBy(r => r.DateFrom)
            .ToList();

        // Korisnici — za prikaz imena u reviewima umesto GUID-a
        var allUsers = await _userService.GetAllAsync();
        ViewBag.Users = allUsers;

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

    // Kreiranje smestaja — upload vise slika, prva u listi je primarna.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Accommodation accommodation, IList<IFormFile>? imageFiles, int primaryIndex = 0)
    {
        var ownerId = HttpContext.GetCurrentUserId();
        if (!HttpContext.IsInRole("Owner") || string.IsNullOrWhiteSpace(ownerId))
            return RedirectToAction("Login", "Account");

        try
        {
            accommodation.OwnerId = ownerId;
            accommodation.ImageUrls = new List<string>();

            if (imageFiles is { Count: > 0 })
            {
                // Upload svih slika
                var uploadedUrls = new List<string>();
                foreach (var file in imageFiles)
                    uploadedUrls.Add(await _fileUploadService.UploadAccommodationImageAsync(file));

                // Primarna slika ide na index 0
                if (primaryIndex >= 0 && primaryIndex < uploadedUrls.Count)
                {
                    var primary = uploadedUrls[primaryIndex];
                    uploadedUrls.RemoveAt(primaryIndex);
                    uploadedUrls.Insert(0, primary);
                }

                accommodation.ImageUrls = uploadedUrls;
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
    public async Task<IActionResult> Edit(string id, Accommodation accommodation, IList<IFormFile>? imageFiles, int primaryIndex = 0, string? keepImages = null)
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

            // keepImages je comma-separated lista URL-ova koje vlasnik zeli da zadrzi
            var retained = keepImages?
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(u => u.Trim())
                .ToList() ?? existingAccommodation.ImageUrls ?? new List<string>();

            // Upload novih slika i dodaj ih na kraj
            if (imageFiles is { Count: > 0 })
            {
                foreach (var file in imageFiles)
                    retained.Add(await _fileUploadService.UploadAccommodationImageAsync(file));
            }

            // Primarna slika ide na index 0
            if (primaryIndex >= 0 && primaryIndex < retained.Count)
            {
                var primary = retained[primaryIndex];
                retained.RemoveAt(primaryIndex);
                retained.Insert(0, primary);
            }

            accommodation.ImageUrls = retained;

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
