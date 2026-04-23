using DTOs.Auth;
using Entities.Extensions;
using Entities.Models;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Repositories.Interfaces;
using Services.Implementations;
using Services.Interfaces.images;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Services.Units;

public class AuthServiceUnitTests
{
    private readonly ITestOutputHelper _output;

    private readonly Mock<IAuthRepository>   _authRepo         = new();
    private readonly Mock<IUserImageService> _userImageService = new();

    private readonly AuthService _svc;

    public AuthServiceUnitTests(ITestOutputHelper output)
    {
        _output = output;

        _userImageService
            .Setup(s => s.GetAvatarUrl(It.IsAny<Guid>()))
            .ReturnsAsync((Guid id) => $"/uploads/avatars/{id}.jpg");

        var jwtSettings = Options.Create(new JwtSettings
        {
            Secret            = "test-secret-key-minimum-16-chars!",
            Issuer            = "test-issuer",
            Audience          = "test-audience",
            ExpirationMinutes = 60
        });

        _svc = new AuthService(_authRepo.Object, jwtSettings, _userImageService.Object);
    }

    // ─────────────────────────────────────────────
    // Register
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Register_ShouldFail_WhenUsernameAlreadyTaken()
    {
        _authRepo.Setup(r => r.UserExistsByUsername("taken")).ReturnsAsync(true);

        var result = await _svc.Register(new RegisterRequest("taken", "new@test.com", "Test User", "Password1!"));

        result.IsSuccess.Should().BeFalse();
        result.ErrorField.Should().Be("username");
        result.Message.Should().Be("Ez a felhasználónév foglalt.");
    }

    [Fact]
    public async Task Register_ShouldFail_WhenEmailAlreadyTaken()
    {
        _authRepo.Setup(r => r.UserExistsByUsername(It.IsAny<string>())).ReturnsAsync(false);
        _authRepo.Setup(r => r.UserExistsByEmail("taken@test.com")).ReturnsAsync(true);

        var result = await _svc.Register(new RegisterRequest("newuser", "Test User","taken@test.com", "Password1!"));

        result.IsSuccess.Should().BeFalse();
        result.ErrorField.Should().Be("email");
        result.Message.Should().Be("Ez az Email Cím már használatban van.");
    }

    [Fact]
    public async Task Register_ShouldSucceed_WhenCredentialsAreUnique()
    {
        _authRepo.Setup(r => r.UserExistsByUsername(It.IsAny<string>())).ReturnsAsync(false);
        _authRepo.Setup(r => r.UserExistsByEmail(It.IsAny<string>())).ReturnsAsync(false);
        _authRepo.Setup(r => r.Create(It.IsAny<User>())).ReturnsAsync((User u) => u);
        _authRepo.Setup(r => r.Update(It.IsAny<User>())).Returns(Task.CompletedTask);

        var result = await _svc.Register(new RegisterRequest("newuser", "new@test.com", "Test User", "Password1!"));

        result.IsSuccess.Should().BeTrue();
        result.Value!.Username.Should().Be("newuser");
        result.Value.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Register_ShouldCallRepositoryCreate_Once()
    {
        _authRepo.Setup(r => r.UserExistsByUsername(It.IsAny<string>())).ReturnsAsync(false);
        _authRepo.Setup(r => r.UserExistsByEmail(It.IsAny<string>())).ReturnsAsync(false);
        _authRepo.Setup(r => r.Create(It.IsAny<User>())).ReturnsAsync((User u) => u);
        _authRepo.Setup(r => r.Update(It.IsAny<User>())).Returns(Task.CompletedTask);

        await _svc.Register(new RegisterRequest("newuser", "new@test.com", "Test User", "Password1!"));

        _authRepo.Verify(r => r.Create(It.IsAny<User>()), Times.Once);
    }

    // ─────────────────────────────────────────────
    // Login
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Login_ShouldFail_WhenEmailNotFound()
    {
        _authRepo.Setup(r => r.GetUserByEmail("missing@test.com")).ReturnsAsync((User?)null);

        var result = await _svc.Login(new LoginRequest("missing@test.com", "Password1!"));

        result.IsSuccess.Should().BeFalse();
        result.ErrorField.Should().Be("email");
        result.Message.Should().Be("Email nem található.");
    }

    [Fact]
    public async Task Login_ShouldFail_WhenPasswordIsWrong()
    {
        var user = CreateUser(password: "CorrectPassword1!");
        _authRepo.Setup(r => r.GetUserByEmail(user.Email)).ReturnsAsync(user);

        var result = await _svc.Login(new LoginRequest(user.Email, "WrongPassword!"));

        result.IsSuccess.Should().BeFalse();
        result.ErrorField.Should().Be("password");
        result.Message.Should().Be("Hibás jelszó");
    }

    [Fact]
    public async Task Login_ShouldSucceed_WhenCredentialsAreCorrect()
    {
        const string password = "CorrectPassword1!";
        var user = CreateUser(password: password);

        _authRepo.Setup(r => r.GetUserByEmail(user.Email)).ReturnsAsync(user);
        _authRepo.Setup(r => r.Update(It.IsAny<User>())).Returns(Task.CompletedTask);

        var result = await _svc.Login(new LoginRequest(user.Email, password));

        result.IsSuccess.Should().BeTrue();
        result.Value!.Token.Should().NotBeNullOrEmpty();
        result.Value.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task Login_ShouldReturnToken_WithCorrectUserId()
    {
        const string password = "CorrectPassword1!";
        var user = CreateUser(password: password);

        _authRepo.Setup(r => r.GetUserByEmail(user.Email)).ReturnsAsync(user);
        _authRepo.Setup(r => r.Update(It.IsAny<User>())).Returns(Task.CompletedTask);

        var result = await _svc.Login(new LoginRequest(user.Email, password));

        result.IsSuccess.Should().BeTrue();
        result.Value!.UserId.Should().Be(user.Id);
    }

    // ─────────────────────────────────────────────
    // UpdateUserInfo
    // ─────────────────────────────────────────────

    [Fact]
    public async Task UpdateUserInfo_ShouldFail_WhenUserNotFound()
    {
        _authRepo.Setup(r => r.GetUserById(It.IsAny<Guid>())).ReturnsAsync((User?)null);

        var result = await _svc.UpdateUserInfo(Guid.NewGuid(), new UpdateUserRequest("x", "x@test.com", "X"));

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Felhasználó nem található");
    }

    [Fact]
    public async Task UpdateUserInfo_ShouldFail_WhenNewEmailAlreadyTaken()
    {
        var user = CreateUser();
        _authRepo.Setup(r => r.GetUserById(user.Id)).ReturnsAsync(user);
        _authRepo.Setup(r => r.UserExistsByEmail("taken@test.com")).ReturnsAsync(true);

        var result = await _svc.UpdateUserInfo(user.Id, new UpdateUserRequest(user.Username, user.FullName, "taken@test.com"));

        result.IsSuccess.Should().BeFalse();
        result.ErrorField.Should().Be("email");
    }

    [Fact]
    public async Task UpdateUserInfo_ShouldFail_WhenNewUsernameAlreadyTaken()
    {
        var user = CreateUser();
        _authRepo.Setup(r => r.GetUserById(user.Id)).ReturnsAsync(user);
        _authRepo.Setup(r => r.UserExistsByEmail(user.Email)).ReturnsAsync(false);
        _authRepo.Setup(r => r.UserExistsByUsername("taken")).ReturnsAsync(true);

        var result = await _svc.UpdateUserInfo(user.Id, new UpdateUserRequest("taken", user.Email, user.FullName));

        result.IsSuccess.Should().BeFalse();
        result.ErrorField.Should().Be("username");
    }

    [Fact]
    public async Task UpdateUserInfo_ShouldSucceed_AndCallUpdate()
    {
        var user = CreateUser();
        _authRepo.Setup(r => r.GetUserById(user.Id)).ReturnsAsync(user);
        _authRepo.Setup(r => r.UserExistsByEmail(It.IsAny<string>())).ReturnsAsync(false);
        _authRepo.Setup(r => r.UserExistsByUsername(It.IsAny<string>())).ReturnsAsync(false);
        _authRepo.Setup(r => r.Update(It.IsAny<User>())).Returns(Task.CompletedTask);

        var result = await _svc.UpdateUserInfo(user.Id, new UpdateUserRequest("newname", "New Name", "new@test.com"));

        result.IsSuccess.Should().BeTrue();
        _authRepo.Verify(r => r.Update(It.Is<User>(u =>
            u.Username == "newname" &&
            u.Email    == "new@test.com"
        )), Times.Once);
    }

    // ─────────────────────────────────────────────
    // ChangePassword
    // ─────────────────────────────────────────────

    [Fact]
    public async Task ChangePassword_ShouldFail_WhenUserNotFound()
    {
        _authRepo.Setup(r => r.GetUserById(It.IsAny<Guid>())).ReturnsAsync((User?)null);

        var result = await _svc.ChangePassword(Guid.NewGuid(), new ChangePasswordRequest("Old1!", "New1!", "New1!"));

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Felhasználó nem található");
    }

    [Fact]
    public async Task ChangePassword_ShouldFail_WhenCurrentPasswordIsWrong()
    {
        var user = CreateUser(password: "CorrectOld1!");
        _authRepo.Setup(r => r.GetUserById(user.Id)).ReturnsAsync(user);

        var result = await _svc.ChangePassword(user.Id, new ChangePasswordRequest("WrongOld1!", "NewPassword1!", "NewPassword1!"));

        result.IsSuccess.Should().BeFalse();
        result.ErrorField.Should().Be("currentPassword");
        result.Message.Should().Be("Hibás aktuális jelszó");
    }

    [Fact]
    public async Task ChangePassword_ShouldFail_WhenNewPasswordSameAsOld()
    {
        const string password = "SamePassword1!";
        var user = CreateUser(password: password);
        _authRepo.Setup(r => r.GetUserById(user.Id)).ReturnsAsync(user);

        var result = await _svc.ChangePassword(user.Id, new ChangePasswordRequest(password, password, password));

        result.IsSuccess.Should().BeFalse();
        result.ErrorField.Should().Be("newPassword");
    }

    [Fact]
    public async Task ChangePassword_ShouldSucceed_WhenPasswordsAreValid()
    {
        const string oldPassword = "OldPassword1!";
        var user = CreateUser(password: oldPassword);
        _authRepo.Setup(r => r.GetUserById(user.Id)).ReturnsAsync(user);
        _authRepo.Setup(r => r.Update(It.IsAny<User>())).Returns(Task.CompletedTask);

        var result = await _svc.ChangePassword(user.Id, new ChangePasswordRequest(oldPassword, "NewPassword1!", "NewPassword1!"));

        result.IsSuccess.Should().BeTrue();
        _authRepo.Verify(r => r.Update(It.IsAny<User>()), Times.Once);
    }

    // ─────────────────────────────────────────────
    // GetUserById
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetUserById_ShouldFail_WhenUserNotFound()
    {
        _authRepo.Setup(r => r.GetUserById(It.IsAny<Guid>())).ReturnsAsync((User?)null);

        var result = await _svc.GetUserById(Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Felhasználó nem található");
    }

    [Fact]
    public async Task GetUserById_ShouldReturnCorrectUserData()
    {
        var user = CreateUser();
        _authRepo.Setup(r => r.GetUserById(user.Id)).ReturnsAsync(user);

        var result = await _svc.GetUserById(user.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value!.UserId.Should().Be(user.Id);
        result.Value.Username.Should().Be(user.Username);
        result.Value.Email.Should().Be(user.Email);
    }

    // ─────────────────────────────────────────────
    // Logout
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Logout_ShouldFail_WhenUserNotFound()
    {
        _authRepo.Setup(r => r.GetUserById(It.IsAny<Guid>())).ReturnsAsync((User?)null);

        var result = await _svc.Logout(Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Felhasználó nem található");
    }

    [Fact]
    public async Task Logout_ShouldSucceed_AndSetIsLoggedInFalse()
    {
        var user = CreateUser();
        user.IsLoggedIn = true;
        User? updated = null;

        _authRepo.Setup(r => r.GetUserById(user.Id)).ReturnsAsync(user);
        _authRepo
            .Setup(r => r.Update(It.IsAny<User>()))
            .Callback<User>(u => updated = u)
            .Returns(Task.CompletedTask);

        var result = await _svc.Logout(user.Id);

        result.IsSuccess.Should().BeTrue();
        updated!.IsLoggedIn.Should().BeFalse();
        updated.CurrentSessionId.Should().BeNull();
    }

    // ─────────────────────────────────────────────
    // Delete
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Delete_ShouldFail_WhenUserNotFound()
    {
        _authRepo.Setup(r => r.GetUserById(It.IsAny<Guid>())).ReturnsAsync((User?)null);

        var result = await _svc.Delete(Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Felhasználó nem található");
    }

    [Fact]
    public async Task Delete_ShouldSucceed_AndCallRepositoryDelete()
    {
        var user = CreateUser();
        _authRepo.Setup(r => r.GetUserById(user.Id)).ReturnsAsync(user);
        _authRepo.Setup(r => r.Delete(user)).ReturnsAsync(true);

        var result = await _svc.Delete(user.Id);

        result.IsSuccess.Should().BeTrue();
        _authRepo.Verify(r => r.Delete(user), Times.Once);
    }

    // ─────────────────────────────────────────────
    // Helper
    // ─────────────────────────────────────────────

    private static User CreateUser(string password = "DefaultPass1!") => new()
    {
        Id           = Guid.NewGuid(),
        Username     = "testuser",
        FullName     = "Teszt Elek",
        Email        = "test@test.com",
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
        Role         = "user",
        Created      = DateTime.UtcNow,
        HasAvatar    = false,
        IsLoggedIn   = false
    };
}
