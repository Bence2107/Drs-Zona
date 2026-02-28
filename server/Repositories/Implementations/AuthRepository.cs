using Context;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories.Implementations;

public class AuthRepository(EfContext context) : IAuthRepository
{
    private readonly DbSet<User> _users = context.Users;

    public async Task<User?> GetUserByEmail(string email)
    {
        return await _users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetUserById(Guid userId)
    {
        return await _users.FindAsync(userId);
    }

    public async Task<User?> GetUserByUsername(string username)
    {
        return await _users
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User> CreateUser(User user)
    {
        _users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task UpdateUser(User user)
    {
        _users.Update(user);
        await context.SaveChangesAsync();
    }

    public async Task<bool> UserExistsByEmail(string email)
    {
        return await _users.AnyAsync(u => u.Email == email);
    }

    public async Task<bool> UserExistsByUsername(string username)
    {
        return await _users.AnyAsync(u => u.Username == username);
    }

    public async Task<bool> DeleteUser(User user)
    {
        try
        {
            _users.Remove(user); 
            var rowsAffected = await context.SaveChangesAsync();
            return rowsAffected > 0;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> CheckIfIdExists(Guid? id)
    {
        return await _users.AnyAsync(u => u.Id == id);
    }
}