using MongoDB.Driver;
using StayFinder.Data;
using StayFinder.Models;

namespace StayFinder.Services;


public sealed class UserService : IUserService
{
    // TODO: Implement user account logic.
    //nastavak 

    private readonly IMongoCollection<User> _users;

    public UserService(MongoDbContext context)
    {
        _users = context.Users;
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
        return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
    }

    // =========================
    // CREATE USER (REGISTER)
    // =========================
    public async Task CreateAsync(User user)
    {
        user.Id ??= Guid.NewGuid().ToString();
        user.CreatedAt = DateTime.UtcNow;

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

        if (user.PasswordHash != password)
            return null;

        return user;
    }
}
