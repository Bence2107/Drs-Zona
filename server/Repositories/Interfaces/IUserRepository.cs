using Entities.Models;

namespace Repositories.Interfaces;

public interface IUsersRepository
{
    User? GetUserById(int id);
    List<User> GetAllUsers();
    void Create(User user);
    void Update(User user);
    void Delete(int id);
    User? GetByUsername(string username);
    User? GetByEmail(string email);
    List<User> GetByRole(string role);
    bool CheckIfIdExists(int id);
    bool CheckIfUsernameExists(string username);
    bool CheckIfEmailExists(string email);
}