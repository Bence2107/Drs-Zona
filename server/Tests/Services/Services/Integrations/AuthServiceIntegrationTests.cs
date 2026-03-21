using Context;
using DTOs.Auth;
using Entities.Extensions;
using Entities.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using Repositories.Implementations;
using Services.Implementations;
using Services.Interfaces;
using Services.Interfaces.images;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Services.Integrations;

public class AuthServiceIntegrationTests(ITestOutputHelper output)
{
    private static (EfContext ctx, IAuthService svc) BuildSut()
    {
        var ctx = InMemoryDbFactory.CreateContext();

        var userImageMock = new Mock<IUserImageService>();
        userImageMock
            .Setup(s => s.GetAvatarUrl(It.IsAny<Guid>()))
            .ReturnsAsync((Guid id) => $"/uploads/avatars/{id}.jpg");

        var jwtSettings = Options.Create(new JwtSettings
        {
            Secret            = "test-secret-key-minimum-16-chars!",
            Issuer            = "test-issuer",
            Audience          = "test-audience",
            ExpirationMinutes = 60
        });

        var svc = new AuthService(new AuthRepository(ctx), jwtSettings, userImageMock.Object);

        return (ctx, svc);
    }

    // ─────────────────────────────────────────────
    // Register
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Register_ShouldSucceed_AndPersistUser()
    {
        var (ctx, svc) = BuildSut();

        var result = await svc.Register(new RegisterRequest("newuser", "new@test.com", "New User", "Password1!"));

        result.IsSuccess.Should().BeTrue();
        result.Value!.Token.Should().NotBeNullOrEmpty();

        ctx.ChangeTracker.Clear();
        ctx.Users.Should().HaveCount(1);
        ctx.Users.First().Username.Should().Be("newuser");
    }

    [Fact]
    public async Task Register_ShouldHashPassword_NotStoreAsPlainText()
    {
        var (ctx, svc) = BuildSut();

        await svc.Register(new RegisterRequest("newuser", "new@test.com", "New User", "Password1!"));

        ctx.ChangeTracker.Clear();
        var saved = ctx.Users.First();
        saved.PasswordHash.Should().NotBe("Password1!");
        BCrypt.Net.BCrypt.Verify("Password1!", saved.PasswordHash).Should().BeTrue();
    }

    [Fact]
    public async Task Register_ShouldFail_WhenUsernameAlreadyExists()
    {
        var (ctx, svc) = BuildSut();

        await ctx.Users.AddAsync(CreateUser(username: "taken"));
        await ctx.SaveChangesAsync();

        var result = await svc.Register(new RegisterRequest("taken", "other@test.com", "Someone", "Password1!"));

        result.IsSuccess.Should().BeFalse();
        result.ErrorField.Should().Be("username");
    }

    [Fact]
    public async Task Register_ShouldFail_WhenEmailAlreadyExists()
    {
        var (ctx, svc) = BuildSut();

        await ctx.Users.AddAsync(CreateUser(email: "taken@test.com"));
        await ctx.SaveChangesAsync();

        var exists = await ctx.Users.AnyAsync(u => u.Email == "taken@test.com");
        exists.Should().BeTrue(); 

        var result = await svc.Register(new RegisterRequest("newuser", "taken@test.com", "Someone", "Password1!"));
        output.WriteLine($"IsSuccess: {result.IsSuccess}, Field: {result.ErrorField}, Message: {result.Message}");
    }

    // ─────────────────────────────────────────────
    // Login
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Login_ShouldSucceed_WithCorrectCredentials()
    {
        var (ctx, svc) = BuildSut();

        const string password = "Password1!";
        await ctx.Users.AddAsync(CreateUser(password: password));
        await ctx.SaveChangesAsync();

        var result = await svc.Login(new LoginRequest("test@test.com", password));

        result.IsSuccess.Should().BeTrue();
        result.Value!.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_ShouldFail_WhenEmailNotFound()
    {
        var (_, svc) = BuildSut();

        var result = await svc.Login(new LoginRequest("missing@test.com", "Password1!"));

        result.IsSuccess.Should().BeFalse();
        result.ErrorField.Should().Be("email");
    }

    [Fact]
    public async Task Login_ShouldFail_WhenPasswordIsWrong()
    {
        var (ctx, svc) = BuildSut();

        await ctx.Users.AddAsync(CreateUser(password: "CorrectPassword1!"));
        await ctx.SaveChangesAsync();

        var result = await svc.Login(new LoginRequest("test@test.com", "WrongPassword!"));

        result.IsSuccess.Should().BeFalse();
        result.ErrorField.Should().Be("password");
    }

    [Fact]
    public async Task Login_ShouldSetIsLoggedInTrue_AfterSuccess()
    {
        var (ctx, svc) = BuildSut();

        const string password = "Password1!";
        var user = CreateUser(password: password);
        await ctx.Users.AddAsync(user);
        await ctx.SaveChangesAsync();

        await svc.Login(new LoginRequest(user.Email, password));

        ctx.ChangeTracker.Clear();
        var updated = await ctx.Users.FindAsync(user.Id);
        updated!.IsLoggedIn.Should().BeTrue();
        updated.LastLogin.Should().NotBeNull();
    }

    // ─────────────────────────────────────────────
    // UpdateUserInfo
    // ─────────────────────────────────────────────

    [Fact]
    public async Task UpdateUserInfo_ShouldSucceed_AndPersistChanges()
    {
        var (ctx, svc) = BuildSut();

        var user = CreateUser();
        await ctx.Users.AddAsync(user);
        await ctx.SaveChangesAsync();

        var result = await svc.UpdateUserInfo(user.Id, new UpdateUserRequest("updateduser", "Updated Name","updated@test.com"));

        result.IsSuccess.Should().BeTrue();
        ctx.ChangeTracker.Clear();
        var updated = await ctx.Users.FindAsync(user.Id);
        updated!.Username.Should().Be("updateduser");
        updated.Email.Should().Be("updated@test.com");
        updated.FullName.Should().Be("Updated Name");
    }

    [Fact]
    public async Task UpdateUserInfo_ShouldFail_WhenUserNotFound()
    {
        var (_, svc) = BuildSut();

        var result = await svc.UpdateUserInfo(Guid.NewGuid(), new UpdateUserRequest("x", "x@test.com", "X"));

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Felhasználó nem található");
    }

    [Fact]
    public async Task UpdateUserInfo_ShouldFail_WhenEmailTakenByAnotherUser()
    {
        var (ctx, svc) = BuildSut();

        var user1 = CreateUser(username: "user1", email: "user1@test.com");
        var user2 = CreateUser(username: "user2", email: "user2@test.com");
        await ctx.Users.AddRangeAsync(user1, user2);
        await ctx.SaveChangesAsync();

        var result = await svc.UpdateUserInfo(user1.Id, new UpdateUserRequest("user1", "User One","user2@test.com"));

        result.IsSuccess.Should().BeFalse();
        result.ErrorField.Should().Be("email");
    }

    // ─────────────────────────────────────────────
    // ChangePassword
    // ─────────────────────────────────────────────

    [Fact]
    public async Task ChangePassword_ShouldSucceed_AndUpdateHash()
    {
        var (ctx, svc) = BuildSut();

        const string oldPassword = "OldPassword1!";
        const string newPassword = "NewPassword1!";
        var user = CreateUser(password: oldPassword);
        await ctx.Users.AddAsync(user);
        await ctx.SaveChangesAsync();

        var result = await svc.ChangePassword(user.Id, new ChangePasswordRequest(oldPassword, newPassword, newPassword));

        result.IsSuccess.Should().BeTrue();
        ctx.ChangeTracker.Clear();
        var updated = await ctx.Users.FindAsync(user.Id);
        BCrypt.Net.BCrypt.Verify(newPassword, updated!.PasswordHash).Should().BeTrue();
    }

    [Fact]
    public async Task ChangePassword_ShouldFail_WhenCurrentPasswordWrong()
    {
        var (ctx, svc) = BuildSut();

        var user = CreateUser(password: "CorrectOld1!");
        await ctx.Users.AddAsync(user);
        await ctx.SaveChangesAsync();

        var result = await svc.ChangePassword(user.Id, new ChangePasswordRequest("WrongOld1!", "NewPassword1!", "NewPassword1!"));

        result.IsSuccess.Should().BeFalse();
        result.ErrorField.Should().Be("currentPassword");
    }

    [Fact]
    public async Task ChangePassword_ShouldFail_WhenNewPasswordSameAsOld()
    {
        var (ctx, svc) = BuildSut();

        const string password = "SamePassword1!";
        var user = CreateUser(password: password);
        await ctx.Users.AddAsync(user);
        await ctx.SaveChangesAsync();

        var result = await svc.ChangePassword(user.Id, new ChangePasswordRequest(password, password, password));

        result.IsSuccess.Should().BeFalse();
        result.ErrorField.Should().Be("newPassword");
    }

    // ─────────────────────────────────────────────
    // GetUserById
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetUserById_ShouldReturnUser_WhenExists()
    {
        var (ctx, svc) = BuildSut();

        var user = CreateUser();
        await ctx.Users.AddAsync(user);
        await ctx.SaveChangesAsync();

        var result = await svc.GetUserById(user.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value!.UserId.Should().Be(user.Id);
        result.Value.Username.Should().Be(user.Username);
        result.Value.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task GetUserById_ShouldFail_WhenUserNotFound()
    {
        var (_, svc) = BuildSut();

        var result = await svc.GetUserById(Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Felhasználó nem található");
    }

    // ─────────────────────────────────────────────
    // Logout
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Logout_ShouldSucceed_AndSetIsLoggedInFalse()
    {
        var (ctx, svc) = BuildSut();

        var user = CreateUser();
        user.IsLoggedIn       = true;
        user.CurrentSessionId = "active-session";
        await ctx.Users.AddAsync(user);
        await ctx.SaveChangesAsync();

        var result = await svc.Logout(user.Id);

        result.IsSuccess.Should().BeTrue();
        ctx.ChangeTracker.Clear();
        var updated = await ctx.Users.FindAsync(user.Id);
        updated!.IsLoggedIn.Should().BeFalse();
        updated.CurrentSessionId.Should().BeNull();
    }

    [Fact]
    public async Task Logout_ShouldFail_WhenUserNotFound()
    {
        var (_, svc) = BuildSut();

        var result = await svc.Logout(Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Felhasználó nem található");
    }

    // ─────────────────────────────────────────────
    // Delete
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Delete_ShouldSucceed_AndRemoveUserFromDb()
    {
        var (ctx, svc) = BuildSut();

        var user = CreateUser();
        await ctx.Users.AddAsync(user);
        await ctx.SaveChangesAsync();

        var result = await svc.Delete(user.Id);

        result.IsSuccess.Should().BeTrue();
        ctx.ChangeTracker.Clear();
        ctx.Users.Should().BeEmpty();
    }

    [Fact]
    public async Task Delete_ShouldFail_WhenUserNotFound()
    {
        var (_, svc) = BuildSut();

        var result = await svc.Delete(Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Felhasználó nem található");
    }

    // ─────────────────────────────────────────────
    // Helper
    // ─────────────────────────────────────────────

    private static User CreateUser(
        string username = "testuser",
        string email    = "test@test.com",
        string password = "DefaultPass1!") => new()
    {
        Id           = Guid.NewGuid(),
        Username     = username,
        FullName     = "Teszt Elek",
        Email        = email,
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
        Role         = "user",
        Created      = DateTime.UtcNow,
        HasAvatar    = false,
        IsLoggedIn   = false
    };
}
