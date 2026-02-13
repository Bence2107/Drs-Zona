using DTOs.Auth;
using Entities.Models;

namespace Services.Interfaces;

public interface IAuthService
{
    Task<ResponseResult<AuthResponse>> RegisterAsync(RegisterRequest request);
    Task<ResponseResult<AuthResponse>> LoginAsync(LoginRequest request);
    Task<ResponseResult<bool>> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
    Task<ResponseResult<User>> GetUserByIdAsync(Guid userId);
    Task<ResponseResult<bool>> LogoutAsync(Guid userId);
    Task UpdateLastActivityAsync(Guid userId);
}