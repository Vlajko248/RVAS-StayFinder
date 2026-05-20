using Microsoft.AspNetCore.Mvc;
using StayFinder.Extensions;
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

    // Gost vidi samo svoje rezervacije preko session korisnika.
    [HttpGet]
    public async Task<IActionResult> MyReservations()
    {
        var userId = HttpContext.GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return RedirectToAction("Login", "Account");

        var reservations = await _reservationService.GetByGuestIdAsync(userId);
        return View(reservations);
    }

    // Vlasnik moze da vidi rezervacije samo za svoj smestaj.
    [HttpGet]
    public async Task<IActionResult> OwnerReservations(string accommodationId)
    {
        var ownerId = HttpContext.GetCurrentUserId();
        if (!HttpContext.IsInRole("Owner") || string.IsNullOrWhiteSpace(ownerId))
            return RedirectToAction("Login", "Account");

        if (string.IsNullOrWhiteSpace(accommodationId))
            return BadRequest("AccommodationId is required.");

        var isOwner = await _accommodationService.IsOwnerOfAccommodationAsync(accommodationId, ownerId);
        if (!isOwner)
            return Forbid();

        var reservations = await _reservationService.GetByAccommodationIdAsync(accommodationId);
        return View(reservations);
    }

    [HttpGet]
    public IActionResult Create(string accommodationId)
    {
        var guestId = HttpContext.GetCurrentUserId();
        if (!HttpContext.IsInRole("Guest") || string.IsNullOrWhiteSpace(guestId))
            return RedirectToAction("Login", "Account");

        if (string.IsNullOrWhiteSpace(accommodationId))
            return BadRequest("AccommodationId is required.");

        var reservation = new Reservation
        {
            AccommodationId = accommodationId
        };

        return View(reservation);
    }

    // Kreiranje rezervacije sa proverom dostupnosti i automatskim racunanjem cene.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Reservation reservation)
    {
        var guestId = HttpContext.GetCurrentUserId();
        if (!HttpContext.IsInRole("Guest") || string.IsNullOrWhiteSpace(guestId))
            return RedirectToAction("Login", "Account");

        if (reservation.DateFrom >= reservation.DateTo)
        {
            ModelState.AddModelError(string.Empty, "Invalid date range.");
            return View(reservation);
        }

        var accommodation = await _accommodationService.GetByIdAsync(reservation.AccommodationId);
        if (accommodation == null)
        {
            ModelState.AddModelError(string.Empty, "Accommodation does not exist.");
            return View(reservation);
        }

        var isAvailable = await _reservationService.IsAccommodationAvailableAsync(
            reservation.AccommodationId,
            reservation.DateFrom,
            reservation.DateTo);

        if (!isAvailable)
        {
            ModelState.AddModelError(string.Empty, "This accommodation is already booked for selected dates.");
            return View(reservation);
        }

        var totalNights = Math.Max(1, (reservation.DateTo.Date - reservation.DateFrom.Date).Days);

        reservation.GuestId = guestId;
        reservation.TotalPrice = totalNights * accommodation.PricePerNight;
        reservation.Status = "Active";

        try
        {
            await _reservationService.CreateAsync(reservation);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(reservation);
        }

        return RedirectToAction(nameof(MyReservations));
    }

    // Brisanje moze gost koji je napravio rezervaciju ili owner smestaja.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var userId = HttpContext.GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return RedirectToAction("Login", "Account");

        if (string.IsNullOrWhiteSpace(id))
            return BadRequest("Id is required.");

        var reservation = await _reservationService.GetByIdAsync(id);
        if (reservation == null)
            return NotFound();

        var isGuestOwner = reservation.GuestId == userId;
        var isAccommodationOwner = HttpContext.IsInRole("Owner") &&
                                   await _accommodationService.IsOwnerOfAccommodationAsync(reservation.AccommodationId, userId);

        if (!isGuestOwner && !isAccommodationOwner)
            return Forbid();

        await _reservationService.DeleteAsync(id);

        if (HttpContext.IsInRole("Owner"))
            return RedirectToAction("Reservations", "Owner");

        return RedirectToAction(nameof(MyReservations));
    }
}