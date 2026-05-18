using Microsoft.AspNetCore.Mvc;
using StayFinder.Services;

namespace StayFinder.Controllers;

public class OwnerController : Controller
{
    // TODO: Add owner dashboard and accommodation management actions.
    //nastavak

    private readonly IAccommodationService _accommodationService;
    private readonly IReservationService _reservationService;

    public OwnerController(
        IAccommodationService accommodationService,
        IReservationService reservationService)
    {
        _accommodationService = accommodationService;
        _reservationService = reservationService;
    }

    // =========================
    // OWNER DASHBOARD (summary)
    // =========================
    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        var role = HttpContext.Session.GetString("Role");
        var ownerId = HttpContext.Session.GetString("UserId");

        if (role != "Owner")
            return Unauthorized("Only owners can access this endpoint.");

        var accommodations = await _accommodationService.GetAllAsync();

        var ownerAccommodations = accommodations
            .Where(a => a.OwnerId == ownerId)
            .ToList();

        return Ok(new
        {
            totalAccommodations = ownerAccommodations.Count,
            accommodations = ownerAccommodations
        });
    }

    // =========================
    // OWNER - GET THEIR RESERVATIONS
    // =========================
    [HttpGet]
    public async Task<IActionResult> Reservations()
    {
        var role = HttpContext.Session.GetString("Role");
        var ownerId = HttpContext.Session.GetString("UserId");

        if (role != "Owner")
            return Unauthorized("Only owners allowed.");

        var accommodations = await _accommodationService.GetAllAsync();

        var ownerAccommodationIds = accommodations
            .Where(a => a.OwnerId == ownerId)
            .Select(a => a.Id)
            .ToList();

        var allReservations = new List<object>();

        foreach (var accId in ownerAccommodationIds)
        {
            var reservations = await _reservationService.GetByAccommodationIdAsync(accId!);
            allReservations.AddRange(reservations);
        }

        return Ok(allReservations);
    }

}
