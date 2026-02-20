using System.Security.Claims;
using DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Interfaces.images;

namespace Drs_Zona.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    IAuthService authService, 
    IUserImageService userImageService)
: ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        var result = await authService.Register(request);
        
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
        var result = await authService.Login(request);
        
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
        var result = await authService.Logout(userId);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Message });
        }

        return Ok(new { message = "Logged out successfully" });
    }
    
    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserProfileResponse>> GetCurrentUser()
    {
        
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        await authService.UpdateLastActivity(userId);

        var result = await authService.GetUserById(userId);

        if (!result.IsSuccess)
            return NotFound(new { message = result.Message });

        return Ok(result.Value);
    }
    
    [Authorize]
    [HttpPost("profile-update")]
    public async Task<IActionResult> UpdateInfo([FromBody] UpdateUserRequest request) {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        
        await authService.UpdateLastActivity(userId);
        
        var result = await authService.UpdateUserInfo(userId, request);

        if (!result.IsSuccess)
        {
            return BadRequest(new 
            { 
                field = result.ErrorField,
                message = result.Message 
            });
        }

        return Ok(new { message = "User infos changed successfully" });
    }
    
    [Authorize]
    [HttpPost("change-password")]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        
        await authService.UpdateLastActivity(userId);
        
        var result = await authService.ChangePassword(userId, request);

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
    [HttpPost("profile-picture-update")]
    public async Task<IActionResult> UpdateProfilePicture([FromForm] ProfilePictureUpload model)
    {
        var file = model.File;
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        
        await authService.UpdateLastActivity(userId);

        var result = await userImageService.SaveAvatar(userId, file);

        if (!result.IsSuccess)
        {
            return BadRequest(new 
            { 
                field = result.ErrorField,
                message = result.Message 
            });
        }

        return Ok(new { message = "User infos changed successfully" });
    }
    
    [Authorize]
    [HttpPost("profile-picture-delete")]
    public async Task<IActionResult> DeleteProfilePicture() {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        
        await authService.UpdateLastActivity(userId);

        var result = userImageService.DeleteAvatar(userId);

        if (!result.IsSuccess)
        {
            return BadRequest(new 
            { 
                field = result.ErrorField,
                message = result.Message 
            });
        }

        return Ok(new { message = "User infos changed successfully" });
    }
}