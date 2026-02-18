using System.Security.Claims;
using DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Drs_Zona.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        var result = await authService.RegisterAsync(request);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new 
            { 
                field = result.ErrorField,
                message = result.Message 
            });
        }

        return Ok(result.Value);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var result = await authService.LoginAsync(request);
        
        if (!result.IsSuccess)
        {
            return Unauthorized(new 
            { 
                field = result.ErrorField,
                message = result.Message 
            });
        }

        return Ok(result.Value);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var result = await authService.LogoutAsync(userId);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Message });
        }

        return Ok(new { message = "Logged out successfully" });
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        
        // Update last activity
        await authService.UpdateLastActivityAsync(userId);
        
        var result = await authService.ChangePasswordAsync(userId, request);

        if (!result.IsSuccess)
        {
            return BadRequest(new 
            { 
                field = result.ErrorField,
                message = result.Message 
            });
        }

        return Ok(new { message = "Password changed successfully" });
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserProfileResponse>> GetCurrentUser()
    {
        
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        await authService.UpdateLastActivityAsync(userId);

        var result = await authService.GetUserByIdAsync(userId);

        if (!result.IsSuccess)
            return NotFound(new { message = result.Message });

        return Ok(result.Value);
    }
}