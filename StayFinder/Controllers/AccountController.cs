using Microsoft.AspNetCore.Mvc;
using StayFinder.Models;
using StayFinder.Services;


namespace StayFinder.Controllers;

public class AccountController : Controller
{
    // TODO: Add login, register, logout, and profile actions.
    /// nastavak 
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
        return Ok("Register endpoint ready");
    }

    // =========================
    // REGISTER (POST)
    // =========================
    [HttpPost]
    public async Task<IActionResult> Register(User user)
    {
        var email = user.Email?.Trim().ToLowerInvariant();
        var password = user.PasswordHash?.Trim();

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return BadRequest("Email and password are required.");

        if (!email.Contains('@'))
            return BadRequest("Email format is invalid.");

        if (password.Length < 6)
            return BadRequest("Password must be at least 6 characters long.");

        var existingUser = await _userService.GetByEmailAsync(email);
        if (existingUser != null)
            return BadRequest("User already exists.");

        user.Email = email;
        user.PasswordHash = password;
        user.Role = "Guest";

        await _userService.CreateAsync(user);

        return Ok("User registered successfully.");
    }

    // =========================
    // LOGIN (POST)
    // =========================
    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return BadRequest("Email and password are required.");

        var user = await _userService.ValidateUserAsync(email, password);

        if (user == null)
            return BadRequest("Invalid email or password.");

        // SESSION LOGIN
        HttpContext.Session.SetString("UserId", user.Id!);
        HttpContext.Session.SetString("Role", user.Role);

        return Ok(new
        {
            message = "Login successful",
            userId = user.Id,
            role = user.Role
        });
    }

    // =========================
    // LOGOUT
    // =========================
    [HttpPost]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return Ok("Logged out successfully.");
    }

    // =========================
    // PROFILE (CURRENT USER)
    // =========================
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var userId = HttpContext.Session.GetString("UserId");

        if (userId == null)
            return Unauthorized("Not logged in.");

        var user = await _userService.GetByIdAsync(userId);

        if (user == null)
            return NotFound("User not found.");

        return Ok(new
        {
            user.Id,
            user.Email,
            user.FullName,
            user.Role,
            user.CreatedAt
        });
    }
}
