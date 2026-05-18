using Microsoft.AspNetCore.Mvc;
using StayFinder.Models;
using StayFinder.Services;

namespace StayFinder.Controllers;

public class ReservationController : Controller
{
    private readonly IReservationService _reservationService;
    private readonly IAccommodationService _accommodationService;

    public ReservationController(
        IReservationService reservationService,
        IAccommodationService accommodationService)
    {
        _reservationService = reservationService;
        _accommodationService = accommodationService;
    }

    // =========================
    // GOST: Moje rezervacije
    // =========================
    public async Task<IActionResult> MyReservations(string guestId)
    {
        if (string.IsNullOrEmpty(guestId))
        {
            return BadRequest("GuestId is required.");
        }

        var reservations =
            await _reservationService.GetByGuestIdAsync(guestId);

        return View(reservations);
    }

    // =========================
    // VLASNIK: rezervacije za smeštaj
    // =========================
    public async Task<IActionResult> OwnerReservations(string accommodationId)
    {
        if (string.IsNullOrEmpty(accommodationId))
        {
            return BadRequest("AccommodationId is required.");
        }

        var reservations =
            await _reservationService.GetByAccommodationIdAsync(accommodationId);

        return View(reservations);
    }

    // =========================
    // kreiranje rezervacije (GET)
    // =========================
    [HttpGet]
    public IActionResult Create(string accommodationId)
    {
        if (string.IsNullOrEmpty(accommodationId))
        {
            return BadRequest("AccommodationId is required.");
        }

        var reservation = new Reservation
        {
            AccommodationId = accommodationId
        };

        return View(reservation);
    }

    // =========================
    // kreiranje rezervacije (POST)
    // =========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Reservation reservation)
    {
        if (reservation.DateFrom >= reservation.DateTo)
        {
            ModelState.AddModelError("", "Invalid date range.");
            return View(reservation);
        }

        var isAvailable =
            await _reservationService.IsAccommodationAvailableAsync(
                reservation.AccommodationId,
                reservation.DateFrom,
                reservation.DateTo);

        if (!isAvailable)
        {
            ModelState.AddModelError("", "This accommodation is already booked for selected dates.");
            return View(reservation);
        }

        await _reservationService.CreateAsync(reservation);

        return RedirectToAction("MyReservations",
            new { guestId = reservation.GuestId });
    }

    // =========================
    // brisanje rezervacije
    // =========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest("Id is required.");
        }

        await _reservationService.DeleteAsync(id);

        return RedirectToAction("MyReservations");
    }
}