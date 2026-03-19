using Entities.Models;

namespace Repositories.Interfaces;

public interface IAuthRepository
{
    Task<User?> GetUserByEmail(string email);
    Task<User?> GetUserById(Guid userId);
    Task<bool> UserExistsByEmail(string email);
    Task<bool> UserExistsByUsername(string username);
    Task<User> Create(User user);
    Task Update(User user);
    Task<bool> Delete(User user);
    Task<bool> CheckIfIdExists(Guid? id);
}