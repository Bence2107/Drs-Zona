using DTOs.Auth;

namespace Services.Interfaces;

public interface IAuthService
{
    Task<ResponseResult<AuthResponse>> Register(RegisterRequest request);
    Task<ResponseResult<AuthResponse>> Login(LoginRequest request);
    Task<ResponseResult<bool>> UpdateUserInfo(Guid userId, UpdateUserRequest request);
    Task<ResponseResult<bool>> ChangePassword(Guid userId, ChangePasswordRequest request);
    Task<ResponseResult<UserProfileResponse>> GetUserById(Guid userId);
    Task<ResponseResult<bool>> Logout(Guid userId);
    Task UpdateLastActivity(Guid userId);
}