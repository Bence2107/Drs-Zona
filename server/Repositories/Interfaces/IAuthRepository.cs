using Entities.Models;

namespace Repositories.Interfaces;

public interface IAuthRepository
{
    Task<User?> GetUserByEmail(string email);
    Task<User?> GetUserById(Guid userId);
    Task<User?> GetUserByUsername(string username);
    Task<User> CreateUser(User user);
    Task UpdateUser(User user);
    Task<bool> UserExistsByEmail(string email);
    Task<bool> UserExistsByUsername(string username);
    Task<bool> DeleteUser(User user);
    bool CheckIfIdExists(Guid id);
}