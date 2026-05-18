using Microsoft.AspNetCore.Mvc;
using StayFinder.Services;

namespace StayFinder.Controllers;

public class AccommodationController : Controller
{
    // TODO: Add accommodation listing and owner management actions.
    //nastavak

    private readonly IAccommodationService _accommodationService;

    public AccommodationController(IAccommodationService accommodationService)
    {
        _accommodationService = accommodationService;
    }

    public async Task<IActionResult> Index()
    {
        var accommodations = await _accommodationService.GetAllAsync();

        return View(accommodations);
    }

    public async Task<IActionResult> Details(string id)
    {
        var accommodation = await _accommodationService.GetByIdAsync(id);

        if (accommodation == null)
        {
            return NotFound();
        }

        return View(accommodation);
    }
}
