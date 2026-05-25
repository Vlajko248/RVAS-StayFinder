using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using StayFinder.Data;
using StayFinder.Models;

namespace StayFinder.Services;

public sealed class UserService : IUserService
{
    // TODO: Implement user account logic.
    //nastavak 

    private readonly IMongoCollection<User> _users;
    private readonly PasswordHasher<User> _passwordHasher = new();

    public UserService(MongoDbContext context)
    {
        _users = context.Users;
    }

    private static string NormalizeEmail(string email)
    {
        return email.Trim().ToLowerInvariant();
    }

    // =========================
    // GET ALL USERS (opciono za debug/admin)
    // =========================
    public async Task<List<User>> GetAllAsync()
    {
        return await _users.Find(_ => true).ToListAsync();
    }

    // =========================
    // GET BY ID
    // =========================
    public async Task<User?> GetByIdAsync(string id)
    {
        return await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
    }

    // =========================
    // GET BY EMAIL (LOGIN KLJUČNO)
    // =========================
    public async Task<User?> GetByEmailAsync(string email)
    {
        var normalizedEmail = NormalizeEmail(email);
        return await _users.Find(u => u.Email == normalizedEmail).FirstOrDefaultAsync();
    }

    // =========================
    // CREATE USER (REGISTER)
    // =========================
    public async Task CreateAsync(User user)
    {
        user.Id ??= Guid.NewGuid().ToString();
        user.Email = NormalizeEmail(user.Email);
        user.CreatedAt = DateTime.UtcNow;
        user.Role = string.IsNullOrWhiteSpace(user.Role) ? "Guest" : user.Role;

        var plainPassword = user.PasswordHash;
        user.PasswordHash = _passwordHasher.HashPassword(user, plainPassword);

        await _users.InsertOneAsync(user);
    }

    // =========================
    // VALIDATE LOGIN (OPCIONO - MOŽE I U CONTROLLERU)
    // =========================
    public async Task<User?> ValidateUserAsync(string email, string password)
    {
        var user = await GetByEmailAsync(email);

        if (user == null)
            return null;

        // Proveravamo da li se lozinka poklapa sa hashom u bazi
        var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

        if (verifyResult == PasswordVerificationResult.Success)
            return user;

        return null;
    }
}
