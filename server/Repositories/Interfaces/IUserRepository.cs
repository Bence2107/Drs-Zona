using Entities.Models;

namespace Repositories.Interfaces;

public interface IUsersRepository
{
    User? GetUserById(Guid id);
    List<User> GetAllUsers();
    void Create(User user);
    void Update(User user);
    void Delete(Guid id);
    User? GetByUsername(string username);
    User? GetByEmail(string email);
    List<User> GetByRole(string role);
    bool CheckIfIdExists(Guid id);
    bool CheckIfUsernameExists(string username);
    bool CheckIfEmailExists(string email);
}