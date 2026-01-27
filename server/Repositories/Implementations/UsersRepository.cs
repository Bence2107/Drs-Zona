using Context;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories.Implementations;

public class UsersRepository(EfContext context) : IUsersRepository
{
    private readonly DbSet<User> _users = context.Users;

    public User? GetUserById(Guid id) => _users.FirstOrDefault(u => u.Id == id);

    public List<User> GetAllUsers() => _users.ToList();

    public void Create(User user)
    {
        _users.Add(user);
        context.SaveChanges();
    }

    public void Update(User user)
    {
        _users.Update(user);
        context.SaveChanges();
    }

    public void Delete(Guid id)
    {
        var user = GetUserById(id);
        if (user == null) return;

        _users.Remove(user);
        context.SaveChanges();
    }

    public User? GetByUsername(string username) => _users.FirstOrDefault(u => u.Username == username);

    public User? GetByEmail(string email) => _users.FirstOrDefault(u => u.Email == email);

    public List<User> GetByRole(string role) => _users
        .Where(u => u.Role == role)
        .ToList();

    public bool CheckIfIdExists(Guid id) => _users.Any(u => u.Id == id);

    public bool CheckIfUsernameExists(string username) => _users.Any(u => u.Username == username);

    public bool CheckIfEmailExists(string email) => _users.Any(u => u.Email == email);
}