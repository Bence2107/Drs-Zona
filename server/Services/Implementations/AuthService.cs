using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DTOs.Auth;
using Entities.Extensions;
using Entities.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Repositories.Interfaces;
using Services.Interfaces;
using Services.Interfaces.images;

namespace Services.Implementations;

public class AuthService(IAuthRepository authRepository, IOptions<JwtSettings> jwtSettings, IUserImageService userImageService)
    : IAuthService
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    public async Task<ResponseResult<AuthResponse>> Register(RegisterRequest request)
    {
        if (await authRepository.UserExistsByEmailAsync(request.Email))
        {
            return ResponseResult<AuthResponse>.Failure(
                field: "email",
                message: "Ez az Email Cím már használatban van."
            );
        }

        if (await authRepository.UserExistsByUsernameAsync(request.Username))
        {
            return ResponseResult<AuthResponse>.Failure(
                field: "username",
                message: "Ez a felhasználónév foglalt."
            );
        }

        try
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = "User",
                Created = DateTime.UtcNow,
                HasAvatar = false,
                IsLoggedIn = false
            };

            await authRepository.CreateUserAsync(user);
            var authResponse = await GenerateAuthResponse(user);

            return ResponseResult<AuthResponse>.Success(authResponse);
        }
        catch (Exception ex)
        {
            return ResponseResult<AuthResponse>.Failure(
                message: $"Registration failed: {ex.Message}"
            );
        }
    }

    public async Task<ResponseResult<AuthResponse>> Login(LoginRequest request)
    {
        var user = await authRepository.GetUserByEmailAsync(request.Email);
        
        if (user is null)
        {
            return ResponseResult<AuthResponse>.Failure(
                field: "email",
                message: "Email nem található."
            );
        }
        
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return ResponseResult<AuthResponse>.Failure(
                field: "password",
                message: "Hibás jelszó"
            );
        }

        try
        {
            user.LastLogin = DateTime.UtcNow;
            user.LastActive = DateTime.UtcNow;
            user.IsLoggedIn = true;
            
            await authRepository.UpdateUserAsync(user);

            var authResponse = await GenerateAuthResponse(user);
            return ResponseResult<AuthResponse>.Success(authResponse);
        }
        catch (Exception ex)
        {
            return ResponseResult<AuthResponse>.Failure(
                message: $"Login failed: {ex.Message}"
            );
        }
    }

    public async Task<ResponseResult<bool>> UpdateUserInfo(Guid userId, UpdateUserRequest request)
    {
        var user = await authRepository.GetUserByIdAsync(userId);
        
        if (user is null)
        {
            return ResponseResult<bool>.Failure(
                message: "Felhasználó nem található"
            );
        }

        if (request.Email != user.Email)
        {
            var exist = await authRepository.UserExistsByEmailAsync(request.Email);
            if (exist)
            {
                return ResponseResult<bool>.Failure(
                    field: "email",
                    message: "Ez az Email Cím már használatban van."
                );
            }
        }

        if (request.Username != user.Username)
        {
            var exist = await authRepository.UserExistsByEmailAsync(request.Email);
            if (exist)
            {
                return ResponseResult<bool>.Failure(
                    field: "username",
                    message: "Ez a felhasználónév foglalt."
                );
            }
        }
        
        try
        {
            user.Email = request.Email;
            user.FullName = request.FullName;
            user.Username = request.Username;

            await authRepository.UpdateUserAsync(user);
            return ResponseResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return ResponseResult<bool>.Failure(
                message: $"User Info changed failed: {ex.Message}"
            );
        }
    }

    public async Task<ResponseResult<bool>> ChangePassword(Guid userId, ChangePasswordRequest request)
    {
        var user = await authRepository.GetUserByIdAsync(userId);
        
        if (user is null)
        {
            return ResponseResult<bool>.Failure(
                message: "Felhasználó nem található"
            );
        }

        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
        {
            return ResponseResult<bool>.Failure(
                field: "currentPassword",
                message: "Hibás aktuális jelszó"
            );
        }

        if (BCrypt.Net.BCrypt.Verify(request.NewPassword, user.PasswordHash))
        {
            return ResponseResult<bool>.Failure(
                field: "newPassword",
                message: "Az új jelszó és a régi jelszónak különbözőnek kell lennie"
            );
        }

        try
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await authRepository.UpdateUserAsync(user);

            return ResponseResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return ResponseResult<bool>.Failure(
                message: $"Password change failed: {ex.Message}"
            );
        }
    }

    public async Task<ResponseResult<UserProfileResponse>> GetUserById(Guid userId)
    {
        
        var user = await authRepository.GetUserByIdAsync(userId);

        if (user is null)
            return ResponseResult<UserProfileResponse>.Failure(message: "Felhasználó nem található");

        return ResponseResult<UserProfileResponse>.Success(new UserProfileResponse
        {
            UserId = user.Id,
            Username = user.Username,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role,
            HasAvatar = user.HasAvatar,
            AvatarUrl = userImageService.GetAvatarUrl(user.Id),
            CreatedAt = user.Created,
        });
    }

    public async Task<ResponseResult<bool>> Logout(Guid userId)
    {
        var user = await authRepository.GetUserByIdAsync(userId);
        
        if (user is null)
        {
            return ResponseResult<bool>.Failure(
                message: "Felhasználó nem található"
            );
        }

        try
        {
            user.LastActive = DateTime.UtcNow;
            user.IsLoggedIn = false;
            user.CurrentSessionId = null;
            
            await authRepository.UpdateUserAsync(user);

            return ResponseResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return ResponseResult<bool>.Failure(
                message: $"Logout failed: {ex.Message}"
            );
        }
    }

    public async Task UpdateLastActivity(Guid userId)
    {
        var user = await authRepository.GetUserByIdAsync(userId);
        if (user != null)
        {
            user.LastActive = DateTime.UtcNow;
            await authRepository.UpdateUserAsync(user);
        }
    }

    private async Task<AuthResponse> GenerateAuthResponse(User user)
    {
        var sessionId = Guid.NewGuid().ToString();
        var token = GenerateJwtToken(user, sessionId);
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);

        user.CurrentSessionId = sessionId;
        user.IsLoggedIn = true;
        await authRepository.UpdateUserAsync(user);

        return new AuthResponse
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            Token = token,
            ExpiresAt = expiresAt
        };
    }

    private string GenerateJwtToken(User user, string sessionId)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, sessionId)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            signingCredentials: cred
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}