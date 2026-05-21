using Microsoft.AspNetCore.Mvc;
using StayFinder.Extensions;
using StayFinder.Models;
using StayFinder.Services;

namespace StayFinder.Controllers;

public class AccountController : Controller
{
    private readonly IUserService _userService;

    public AccountController(IUserService userService)
    {
        _userService = userService;
    }

    // =========================
    // REGISTER (GET)
    // =========================
    [HttpGet]
    public IActionResult Register()
    {
        return View(new User { Role = "Guest" });
    }

    // =========================
    // REGISTER (POST)
    // =========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(User user)
    {
        var email = user.Email?.Trim().ToLowerInvariant();
        var password = user.PasswordHash?.Trim();
        var fullName = user.FullName?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError(string.Empty, "Email and password are required.");
            return View(user);
        }

        if (!email.Contains('@'))
        {
            ModelState.AddModelError(nameof(user.Email), "Email format is invalid.");
            return View(user);
        }

        if (password.Length < 6)
        {
            ModelState.AddModelError(nameof(user.PasswordHash), "Password must be at least 6 characters long.");
            return View(user);
        }

        if (!string.Equals(user.Role, "Owner", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(user.Role, "Guest", StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(nameof(user.Role), "Role must be Owner or Guest.");
            return View(user);
        }

        var existingUser = await _userService.GetByEmailAsync(email);
        if (existingUser != null)
        {
            ModelState.AddModelError(nameof(user.Email), "User already exists.");
            return View(user);
        }

        user.Email = email;
        user.PasswordHash = password;
        user.FullName = fullName;
        user.Role = string.Equals(user.Role, "Owner", StringComparison.OrdinalIgnoreCase)
            ? "Owner"
            : "Guest";

        await _userService.CreateAsync(user);
        TempData["Message"] = "User registered successfully. Please login.";

        return RedirectToAction(nameof(Login));
    }

    // =========================
    // LOGIN (GET)
    // =========================
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // =========================
    // LOGIN (POST)
    // =========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError(string.Empty, "Email and password are required.");
            return View();
        }

        var user = await _userService.ValidateUserAsync(email, password);

        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View();
        }

        // SESSION LOGIN
        HttpContext.Session.SetString("UserId", user.Id!);
        HttpContext.Session.SetString("Role", user.Role);

        if (HttpContext.IsInRole("Owner"))
            return RedirectToAction("Dashboard", "Owner");

        return RedirectToAction("Index", "Home");
    }

    // =========================
    // LOGOUT
    // =========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }

    // =========================
    // PROFILE (CURRENT USER)
    // =========================
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var userId = HttpContext.GetCurrentUserId();

        if (userId == null)
            return RedirectToAction(nameof(Login));

        var user = await _userService.GetByIdAsync(userId);

        if (user == null)
            return NotFound("User not found.");

        user.PasswordHash = string.Empty;
        return View(user);
    }
}
