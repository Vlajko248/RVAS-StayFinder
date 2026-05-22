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
        // var guestId = HttpContext.GetCurrentUserId();
        // if (!HttpContext.IsInRole("Guest") || string.IsNullOrWhiteSpace(guestId))
        //     return RedirectToAction("Login", "Account");
        var guestId = HttpContext.Session.GetString("UserId");
        var role = HttpContext.Session.GetString("Role");

        if (role != "Guest" || string.IsNullOrWhiteSpace(guestId))
            return RedirectToAction("Login", "Account");

        var reviews = await _reviewService.GetByGuestIdAsync(guestId);
        return View(reviews);
    }

    // // Recenzija moze samo ako su pravila iz servisa ispunjena.
    // [HttpPost]
    // [ValidateAntiForgeryToken]
    // public async Task<IActionResult> Create(string accommodationId, int rating, string comment)
    // {
    //     var guestId = HttpContext.GetCurrentUserId();
    //     if (!HttpContext.IsInRole("Guest") || string.IsNullOrWhiteSpace(guestId))
    //         return RedirectToAction("Login", "Account");
    

    //     var review = new Review
    //     {
    //         AccommodationId = accommodationId,
    //         GuestId = guestId,
    //         Rating = rating,
    //         Comment = comment
    //     };

    //     try
    //     {
    //         await _reviewService.CreateAsync(review);
    //     }
    //     catch (InvalidOperationException ex)
    //     {
    //         TempData["ReviewError"] = ex.Message;
    //         return RedirectToAction("Details", "Accommodation", new { id = accommodationId });
    //     }

    //     TempData["ReviewSuccess"] = "Review created.";
    //     return RedirectToAction("Details", "Accommodation", new { id = accommodationId });
    // }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string accommodationId, int rating, string comment)
    {

        var guestId = HttpContext.Session.GetString("UserId");
        var role = HttpContext.Session.GetString("Role");

        if (role != "Guest" || string.IsNullOrWhiteSpace(guestId))
            return RedirectToAction("Login", "Account");

        var review = new Review
        {
            Id = Guid.NewGuid().ToString(),
            AccommodationId = accommodationId,
            GuestId = guestId,
            Rating = rating,
            Comment = comment,
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            await _reviewService.CreateAsync(review);
        }
        catch (Exception ex)
        {
            Console.WriteLine("REVIEW CREATE ERROR: " + ex.Message);

            TempData["ReviewError"] = ex.Message;
            return RedirectToAction("Details", "Accommodation", new { id = accommodationId });
        }

        TempData["ReviewSuccess"] = "Review created.";
        return RedirectToAction("Details", "Accommodation", new { id = accommodationId });
    }
}
