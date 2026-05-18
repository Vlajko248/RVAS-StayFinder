using StayFinder.Models;

namespace StayFinder.Services;


public interface IUserService
{
    // TODO: Add user authentication and profile operations.
    //nastavak
    Task<List<User>> GetAllAsync();

    Task<User?> GetByIdAsync(string id);

    Task<User?> GetByEmailAsync(string email);

    Task CreateAsync(User user);

    Task<User?> ValidateUserAsync(string email, string password);
}
