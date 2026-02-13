using Context;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories.Implementations;

public class AuthRepository(EfContext context) : IAuthRepository
{
    private readonly DbSet<User> _users = context.Users;

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        return await _users.FindAsync(userId);
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _users
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User> CreateUserAsync(User user)
    {
        _users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task UpdateUserAsync(User user)
    {
        _users.Update(user);
        await context.SaveChangesAsync();
    }

    public async Task<bool> UserExistsByEmailAsync(string email)
    {
        return await _users.AnyAsync(u => u.Email == email);
    }

    public async Task<bool> UserExistsByUsernameAsync(string username)
    {
        return await _users.AnyAsync(u => u.Username == username);
    }

    public bool CheckIfIdExists(Guid id)
    {
        return _users.Any(u => u.Id == id);
    }
}