using Microsoft.AspNetCore.Mvc;
using StayFinder.Extensions;
using StayFinder.Models;
using StayFinder.Services;

namespace StayFinder.Controllers;

public class OwnerController : Controller
{
    private readonly IAccommodationService _accommodationService;
    private readonly IReservationService _reservationService;

    public OwnerController(
        IAccommodationService accommodationService,
        IReservationService reservationService)
    {
        _accommodationService = accommodationService;
        _reservationService = reservationService;
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        var ownerId = HttpContext.GetCurrentUserId();
        if (!HttpContext.IsInRole("Owner") || string.IsNullOrWhiteSpace(ownerId))
            return RedirectToAction("Login", "Account");

        var ownerAccommodations = await _accommodationService.GetByOwnerIdAsync(ownerId);

        var reservationCount = 0;
        foreach (var accommodation in ownerAccommodations)
        {
            var reservations = await _reservationService.GetByAccommodationIdAsync(accommodation.Id!);
            reservationCount += reservations.Count;
        }

        var model = new OwnerDashboardViewModel
        {
            TotalAccommodations = ownerAccommodations.Count,
            TotalReservations = reservationCount,
            Accommodations = ownerAccommodations
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Reservations()
    {
        var ownerId = HttpContext.GetCurrentUserId();
        if (!HttpContext.IsInRole("Owner") || string.IsNullOrWhiteSpace(ownerId))
            return RedirectToAction("Login", "Account");

        var ownerAccommodations = await _accommodationService.GetByOwnerIdAsync(ownerId);
        var ownerAccommodationIds = ownerAccommodations
            .Where(a => !string.IsNullOrWhiteSpace(a.Id))
            .Select(a => a.Id!)
            .ToHashSet();

        var allReservations = new List<Reservation>();

        foreach (var accId in ownerAccommodationIds)
        {
            var reservations = await _reservationService.GetByAccommodationIdAsync(accId);
            allReservations.AddRange(reservations);
        }

        allReservations = allReservations
            .OrderByDescending(r => r.CreatedAt)
            .ToList();

        return View(allReservations);
    }

    public sealed class OwnerDashboardViewModel
    {
        public int TotalAccommodations { get; set; }
        public int TotalReservations { get; set; }
        public List<Accommodation> Accommodations { get; set; } = new();
    }
}
