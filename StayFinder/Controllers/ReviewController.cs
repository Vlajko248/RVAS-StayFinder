using Microsoft.AspNetCore.Mvc;
using StayFinder.Extensions;
using StayFinder.Models;
using StayFinder.Services;

namespace StayFinder.Controllers;

public class ReviewController : Controller
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    // Javni prikaz recenzija za jedan smestaj.
    [HttpGet]
    public async Task<IActionResult> ForAccommodation(string accommodationId)
    {
        if (string.IsNullOrWhiteSpace(accommodationId))
            return BadRequest("AccommodationId is required.");

        var reviews = await _reviewService.GetByAccommodationIdAsync(accommodationId);
        return View(reviews);
    }

    // Gost vidi samo svoje recenzije.
    [HttpGet]
    public async Task<IActionResult> MyReviews()
    {
        var guestId = HttpContext.GetCurrentUserId();
        if (!HttpContext.IsInRole("Guest") || string.IsNullOrWhiteSpace(guestId))
            return RedirectToAction("Login", "Account");

        var reviews = await _reviewService.GetByGuestIdAsync(guestId);
        return View(reviews);
    }

    // Kreiranje recenzije — gost mora biti ulogovan, ne moze dva puta oceniti isti smestaj.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string accommodationId, int rating, string comment)
    {
        var guestId = HttpContext.GetCurrentUserId();
        if (!HttpContext.IsInRole("Guest") || string.IsNullOrWhiteSpace(guestId))
            return RedirectToAction("Login", "Account");

        var review = new Review
        {
            AccommodationId = accommodationId,
            GuestId = guestId,
            Rating = rating,
            Comment = comment
        };

        try
        {
            await _reviewService.CreateAsync(review);
        }
        catch (InvalidOperationException ex)
        {
            TempData["ReviewError"] = ex.Message;
            return RedirectToAction("Details", "Accommodation", new { id = accommodationId });
        }

        TempData["ReviewSuccess"] = "Review created.";
        return RedirectToAction("Details", "Accommodation", new { id = accommodationId });
    }
}
