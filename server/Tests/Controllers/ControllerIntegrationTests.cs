using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Context;
using DTOs.Auth;
using DTOs.News;
using DTOs.Standings;
using Entities.Extensions;
using Entities.Models;
using Entities.Models.News;
using Entities.Models.Polls;
using Entities.Models.Standings;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace Tests.Controllers;

// ─────────────────────────────────────────────────────────────
// WebApplicationFactory
// ─────────────────────────────────────────────────────────────

public class TestWebAppFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Remove real DB
            services.RemoveAll<DbContextOptions<EfContext>>();
            services.RemoveAll<EfContext>();

            // Add InMemory DB
            services.AddDbContext<EfContext>(opts =>
                opts.UseInMemoryDatabase(_dbName));

            // Override JWT settings
            const string testKey = "nagyon-hosszu-es-biztonsagos-teszt-kulcs-12345";

            services.RemoveAll<IOptions<JwtSettings>>();
            services.AddSingleton(Options.Create(new JwtSettings
            {
                Secret = testKey,
                Issuer = "test-issuer",
                Audience = "test-audience",
                ExpirationMinutes = 60
            }));

            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters.IssuerSigningKey =
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(testKey));
                options.TokenValidationParameters.ValidIssuer = "test-issuer";
                options.TokenValidationParameters.ValidAudience = "test-audience";
                options.TokenValidationParameters.ValidateIssuer = true;
                options.TokenValidationParameters.ValidateAudience = true;
            });
        });
    }
}

// ─────────────────────────────────────────────────────────────
// Base class with helpers
// ─────────────────────────────────────────────────────────────

public abstract class ControllerTestBase : IClassFixture<TestWebAppFactory>
{
    protected readonly HttpClient Client;
    protected readonly TestWebAppFactory Factory;

    protected ControllerTestBase(TestWebAppFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    protected EfContext GetDb()
    {
        var scope = Factory.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<EfContext>();
    }

    /// <summary>Registers a user, logs in, and attaches the JWT to the client.</summary>
    protected async Task<User> AuthenticateAs(string role = "user")
    {
        var db = GetDb();
        var password = "TestPass1!";
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = $"user_{Guid.NewGuid():N}",
            FullName = "Test User",
            Email = $"{Guid.NewGuid()}@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = role,
            Created = DateTime.UtcNow
        };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var loginResp = await Client.PostAsJsonAsync("/api/auth/login",
            new LoginRequest(user.Email, password));
        var auth = await loginResp.Content.ReadFromJsonAsync<AuthResponse>();
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", auth!.Token);

        return user;
    }

    protected static User CreateUser(string role = "user", string? email = null) => new()
    {
        Id = Guid.NewGuid(),
        Username = $"u_{Guid.NewGuid():N}",
        FullName = "Test",
        Email = email ?? $"{Guid.NewGuid()}@test.com",
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("TestPass1!"),
        Role = role,
        Created = DateTime.UtcNow
    };
}

// ─────────────────────────────────────────────────────────────
// Auth Controller Tests
// ─────────────────────────────────────────────────────────────

public class AuthControllerTests(TestWebAppFactory factory) : ControllerTestBase(factory)
{
    [Fact]
    public async Task Register_ShouldReturn200_WithToken()
    {
        var uniqueId = Guid.NewGuid().ToString("N");
        var resp = await Client.PostAsJsonAsync("/api/auth/register",
            new RegisterRequest("newUser2107", "New User", $"{uniqueId}@test.com", "Password123!"));

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Register_ShouldReturn400_WhenUsernameTaken()
    {
        var db = GetDb();
        db.Users.Add(CreateUser(email: "taken@test.com"));
        await db.SaveChangesAsync();

        // Register with same username twice
        await Client.PostAsJsonAsync("/api/auth/register",
            new RegisterRequest("takenuser", "first@test.com", "First", "Password1!"));
        var resp = await Client.PostAsJsonAsync("/api/auth/register",
            new RegisterRequest("takenuser", "second@test.com", "Second", "Password1!"));

        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_ShouldReturn200_WithToken()
    {
        var db = GetDb();
        var password = "LoginPass1!";
        var user = CreateUser();
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var resp = await Client.PostAsJsonAsync("/api/auth/login",
            new LoginRequest(user.Email, password));

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await resp.Content.ReadFromJsonAsync<AuthResponse>();
        body!.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_ShouldReturn401_WhenEmailNotFound()
    {
        var resp = await Client.PostAsJsonAsync("/api/auth/login",
            new LoginRequest("nobody@test.com", "Password1!"));

        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_ShouldReturn401_WhenPasswordWrong()
    {
        var db = GetDb();
        var user = CreateUser();
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPass1!");
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var resp = await Client.PostAsJsonAsync("/api/auth/login",
            new LoginRequest(user.Email, "WrongPass1!"));

        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMe_ShouldReturn200_WhenAuthenticated()
    {
        await AuthenticateAs();

        var resp = await Client.GetAsync("/api/auth/me");

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetMe_ShouldReturn401_WhenNotAuthenticated()
    {
        Client.DefaultRequestHeaders.Authorization = null;

        var resp = await Client.GetAsync("/api/auth/me");

        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Logout_ShouldReturn200_WhenAuthenticated()
    {
        await AuthenticateAs();

        var resp = await Client.PostAsync("/api/auth/logout", null);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ChangePassword_ShouldReturn200_WhenValid()
    {
        await AuthenticateAs();

        var resp = await Client.PostAsJsonAsync("/api/auth/change-password",
            new ChangePasswordRequest("TestPass1!", "NewPass1!", "NewPass1!"));

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ChangePassword_ShouldReturn400_WhenCurrentPasswordWrong()
    {
        await AuthenticateAs();

        var resp = await Client.PostAsJsonAsync("/api/auth/change-password",
            new ChangePasswordRequest("WrongOld1!", "NewPass1!", "NewPass1!"));

        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateInfo_ShouldReturn200_WhenValid()
    {
        await AuthenticateAs();

        var resp = await Client.PostAsJsonAsync("/api/auth/profile-update",
            new UpdateUserRequest("updateduser", "Updated Name", "updated@newtest.com"));

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}

// ─────────────────────────────────────────────────────────────
// HealthCheck Controller Tests
// ─────────────────────────────────────────────────────────────

public class HealthCheckControllerTests(TestWebAppFactory factory) : ControllerTestBase(factory)
{
    [Fact]
    public async Task Ping_ShouldReturn200()
    {
        var resp = await Client.GetAsync("/api/healthcheck/ping");

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}

// ─────────────────────────────────────────────────────────────
// Brands Controller Tests
// ─────────────────────────────────────────────────────────────

public class BrandsControllerTests(TestWebAppFactory factory) : ControllerTestBase(factory)
{
    [Fact]
    public async Task GetAll_ShouldReturn401_WhenNotAuthenticated()
    {
        Client.DefaultRequestHeaders.Authorization = null;
        var resp = await Client.GetAsync("/api/brands/getAll");
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAll_ShouldReturn200_WhenAdminAuthenticated()
    {
        await AuthenticateAs("Admin");
        var resp = await Client.GetAsync("/api/brands/getAll");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAll_ShouldReturnBrands_WhenExist()
    {
        await AuthenticateAs("Admin");
        var db = GetDb();
        db.Brands.AddRange(
            new Brand { Id = Guid.NewGuid(), Name = "Ferrari", Description = "d", Principal = "p", HeadQuarters = "h" },
            new Brand { Id = Guid.NewGuid(), Name = "Mercedes", Description = "d", Principal = "p", HeadQuarters = "h" }
        );
        await db.SaveChangesAsync();

        var resp = await Client.GetAsync("/api/brands/getAll");

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}

// ─────────────────────────────────────────────────────────────
// Contracts Controller Tests
// ─────────────────────────────────────────────────────────────

public class ContractsControllerTests(TestWebAppFactory factory) : ControllerTestBase(factory)
{
    [Fact]
    public async Task GetAll_ShouldReturn401_WhenNotAuthenticated()
    {
        Client.DefaultRequestHeaders.Authorization = null;
        var resp = await Client.GetAsync("/api/contracts/getAll");
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAll_ShouldReturn200_WhenAdmin()
    {
        await AuthenticateAs("Admin");
        var resp = await Client.GetAsync("/api/contracts/getAll");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_ShouldReturn200_WhenAdmin()
    {
        await AuthenticateAs("Admin");
        var dto = new ContractCreateDto(Guid.NewGuid(), Guid.NewGuid());

        var resp = await Client.PostAsJsonAsync("/api/contracts/create", dto);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Delete_ShouldReturn401_WhenNotAuthenticated()
    {
        Client.DefaultRequestHeaders.Authorization = null;
        var resp = await Client.DeleteAsync($"/api/contracts/delete/{Guid.NewGuid()}");
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Delete_ShouldReturn400_WhenContractNotFound()
    {
        await AuthenticateAs("Admin");
        var resp = await Client.DeleteAsync($"/api/contracts/delete/{Guid.NewGuid()}");
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}

// ─────────────────────────────────────────────────────────────
// Drivers Controller Tests
// ─────────────────────────────────────────────────────────────

public class DriversControllerTests(TestWebAppFactory factory) : ControllerTestBase(factory)
{
    [Fact]
    public async Task Get_ShouldReturn401_WhenNotAuthenticated()
    {
        Client.DefaultRequestHeaders.Authorization = null;
        var resp = await Client.GetAsync($"/api/drivers/get/{Guid.NewGuid()}");
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Get_ShouldReturn400_WhenDriverNotFound()
    {
        await AuthenticateAs("Admin");
        var resp = await Client.GetAsync($"/api/drivers/get/{Guid.NewGuid()}");
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Get_ShouldReturn200_WhenDriverExists()
    {
        await AuthenticateAs("Admin");
        var db = GetDb();
        var driver = new Driver
        {
            Id = Guid.NewGuid(),
            Name = "Max Verstappen",
            Nationality = "Dutch",
            BirthDate = new DateTime(1997, 9, 30)
        };
        db.Drivers.Add(driver);
        await db.SaveChangesAsync();

        var resp = await Client.GetAsync($"/api/drivers/get/{driver.Id}");

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAllDrivers_ShouldReturn200_WhenAdmin()
    {
        await AuthenticateAs("Admin");
        var resp = await Client.GetAsync("/api/drivers/getAllDrivers");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_ShouldReturn200_WhenValid()
    {
        await AuthenticateAs("Admin");
        var dto = new DriverCreateDto(
            "Lewis Hamilton", "British",
            new DateTime(1985, 1, 7),
            300, 103, 191, 7, 104, 17
        );

        var resp = await Client.PostAsJsonAsync("/api/drivers/create", dto);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_ShouldReturn400_WhenDriverTooYoung()
    {
        await AuthenticateAs("Admin");
        var dto = new DriverCreateDto(
            "Young Driver", "HU",
            DateTime.Today.AddYears(-10),
            0, 0, 0, 0, 0, 1
        );

        var resp = await Client.PostAsJsonAsync("/api/drivers/create", dto);

        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_ShouldReturn401_WhenNotAuthenticated()
    {
        Client.DefaultRequestHeaders.Authorization = null;
        var dto = new DriverCreateDto("Name", "HU", DateTime.Today.AddYears(-25), 0, 0, 0, 0, 0, 1);

        var resp = await Client.PostAsJsonAsync("/api/drivers/create", dto);

        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}

// ─────────────────────────────────────────────────────────────
// Constructors Controller Tests
// ─────────────────────────────────────────────────────────────

public class ConstructorsControllerTests(TestWebAppFactory factory) : ControllerTestBase(factory)
{
    [Fact]
    public async Task GetAllConstructors_ShouldReturn401_WhenNotAuthenticated()
    {
        Client.DefaultRequestHeaders.Authorization = null;
        var resp = await Client.GetAsync("/api/constructors/getAllConstructors");
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAllConstructors_ShouldReturn200_WhenAdmin()
    {
        await AuthenticateAs("Admin");
        var resp = await Client.GetAsync("/api/constructors/getAllConstructors");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Get_ShouldReturn400_WhenConstructorNotFound()
    {
        await AuthenticateAs("Admin");
        var resp = await Client.GetAsync($"/api/constructors/get/{Guid.NewGuid()}");
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_ShouldReturn200_WhenValid()
    {
        await AuthenticateAs("Admin");
        var db = GetDb();
        var brand = new Brand
            { Id = Guid.NewGuid(), Name = "TestBrand", Description = "d", Principal = "p", HeadQuarters = "h" };
        db.Brands.Add(brand);
        await db.SaveChangesAsync();

        var dto = new ConstructorCreateDto(brand.Id, "New Team", "NT", 2010,
            "London", "Chief", "Tech", 0, 0, 0, 1);

        var resp = await Client.PostAsJsonAsync("/api/constructors/create", dto);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_ShouldReturn401_WhenNotAuthenticated()
    {
        Client.DefaultRequestHeaders.Authorization = null;
        var dto = new ConstructorCreateDto(Guid.NewGuid(), "Team", "T", 2000, "HQ", "C", "T", 0, 0, 0, 1);

        var resp = await Client.PostAsJsonAsync("/api/constructors/create", dto);

        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}

// ─────────────────────────────────────────────────────────────
// Comments Controller Tests
// ─────────────────────────────────────────────────────────────

public class CommentsControllerTests(TestWebAppFactory factory) : ControllerTestBase(factory)
{
    private async Task<(User user, Article article, Comment comment)> SeedCommentData()
    {
        var db = GetDb();
        var user = CreateUser();
        var article = new Article
        {
            Id = Guid.NewGuid(),
            Title = "Test Article",
            Slug = $"test-{Guid.NewGuid():N}",
            Lead = "Lead",
            Tag = "F1",
            FirstSection = "First",
            LastSection = "Last",
            DatePublished = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow,
            AuthorId = user.Id
        };
        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            ArticleId = article.Id,
            UserId = user.Id,
            Content = "Test comment",
            DateCreated = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow,
            User = user
        };
        db.Users.Add(user);
        db.Articles.Add(article);
        db.Comments.Add(comment);
        await db.SaveChangesAsync();
        return (user, article, comment);
    }

    [Fact]
    public async Task GetCommentsWithoutReplies_ShouldReturn200()
    {
        var (_, article, _) = await SeedCommentData();

        var resp = await Client.GetAsync($"/api/comments/getCommentsWithoutReplies/{article.Id}");

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCommentReplies_ShouldReturn200()
    {
        var (_, _, comment) = await SeedCommentData();

        var resp = await Client.GetAsync($"/api/comments/getCommentReplies/{comment.Id}");

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetUsersComments_ShouldReturn401_WhenNotAuthenticated()
    {
        Client.DefaultRequestHeaders.Authorization = null;
        var resp = await Client.GetAsync($"/api/comments/getUsersComments/{Guid.NewGuid()}");
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUsersComments_ShouldReturn200_WhenAuthenticated()
    {
        var user = await AuthenticateAs();
        var resp = await Client.GetAsync($"/api/comments/getUsersComments/{user.Id}");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_ShouldReturn401_WhenNotAuthenticated()
    {
        Client.DefaultRequestHeaders.Authorization = null;
        var dto = new CommentCreateDto(Guid.NewGuid(), null, "Content");
        var resp = await Client.PostAsJsonAsync($"/api/comments/create/{Guid.NewGuid()}", dto);
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Delete_ShouldReturn401_WhenNotAuthenticated()
    {
        Client.DefaultRequestHeaders.Authorization = null;
        var resp = await Client.DeleteAsync($"/api/comments/delete/{Guid.NewGuid()}");
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Delete_ShouldReturn404_WhenCommentNotFound()
    {
        await AuthenticateAs();
        var resp = await Client.DeleteAsync($"/api/comments/delete/{Guid.NewGuid()}");
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

// ─────────────────────────────────────────────────────────────
// GrandPrix Controller Tests
// ─────────────────────────────────────────────────────────────

public class GrandPrixControllerTests(TestWebAppFactory factory) : ControllerTestBase(factory)
{
    [Fact]
    public async Task GetGrandPrixById_ShouldReturn400_WhenNotFound()
    {
        var resp = await Client.GetAsync($"/api/grandprix/get/{Guid.NewGuid()}");
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetAllCircuits_ShouldReturn401_WhenNotAuthenticated()
    {
        Client.DefaultRequestHeaders.Authorization = null;
        var resp = await Client.GetAsync("/api/grandprix/getAllCircuits");
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAllCircuits_ShouldReturn200_WhenAdmin()
    {
        await AuthenticateAs("Admin");
        var resp = await Client.GetAsync("/api/grandprix/getAllCircuits");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetSeasonGrandsPrix_ShouldReturn200()
    {
        var resp = await Client.GetAsync($"/api/grandprix/getSeasonGrandsPrix/{Guid.NewGuid()}/2024");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_ShouldReturn401_WhenNotAuthenticated()
    {
        Client.DefaultRequestHeaders.Authorization = null;
        var resp = await Client.PostAsJsonAsync("/api/grandprix/create", new { });
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}

// ─────────────────────────────────────────────────────────────
// Series Controller Tests
// ─────────────────────────────────────────────────────────────

public class SeriesControllerTests(TestWebAppFactory factory) : ControllerTestBase(factory)
{
    [Fact]
    public async Task GetAllSeries_ShouldReturn200()
    {
        var resp = await Client.GetAsync("/api/series/getAllSeries");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetSeriesByName_ShouldReturn400_WhenNotFound()
    {
        var resp = await Client.GetAsync("/api/series/name/NonExistentSeries");
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetSeriesByName_ShouldReturn200_WhenExists()
    {
        var db = GetDb();
        db.Series.Add(new Series
        {
            Id = Guid.NewGuid(), Name = "UniqueSeriesXYZ", ShortName = "USN", Description = "d", GoverningBody = "FIA",
            FirstYear = 1950, LastYear = 2025, PointSystem = "FIA"
        });
        await db.SaveChangesAsync();

        var resp = await Client.GetAsync("/api/series/name/USN");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}

// ─────────────────────────────────────────────────────────────
// Polls Controller Tests
// ─────────────────────────────────────────────────────────────

public class PollsControllerTests(TestWebAppFactory factory) : ControllerTestBase(factory)
{
    private async Task<(User author, Poll poll, PollOption option)> SeedPollData()
    {
        var db = GetDb();
        var author = CreateUser();
        var poll = new Poll
        {
            Id = Guid.NewGuid(),
            AuthorId = author.Id,
            Title = "Test Poll",
            Tag = "F1",
            Description = "desc",
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.Now.AddDays(7),
            IsActive = true
        };
        var option = new PollOption { Id = Guid.NewGuid(), PollId = poll.Id, Text = "Option A" };
        db.Users.Add(author);
        db.Polls.Add(poll);
        db.PollOptions.Add(option);
        await db.SaveChangesAsync();
        return (author, poll, option);
    }

    [Fact]
    public async Task GetById_ShouldReturn400_WhenNotFound()
    {
        var resp = await Client.GetAsync($"/api/poll/get/{Guid.NewGuid()}");
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetById_ShouldReturn200_WhenExists()
    {
        var (_, poll, _) = await SeedPollData();

        var resp = await Client.GetAsync($"/api/poll/get/{poll.Id}");

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAllActive_ShouldReturn200()
    {
        var resp = await Client.GetAsync("/api/poll/getAllActive");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetByCreatorId_ShouldReturn401_WhenNotAuthenticated()
    {
        Client.DefaultRequestHeaders.Authorization = null;
        var resp = await Client.GetAsync($"/api/poll/getByCreatorId/{Guid.NewGuid()}");
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetByCreatorId_ShouldReturn200_WhenAuthenticated()
    {
        var user = await AuthenticateAs();
        var resp = await Client.GetAsync($"/api/poll/getByCreatorId/{user.Id}");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Delete_ShouldReturn401_WhenNotAuthenticated()
    {
        Client.DefaultRequestHeaders.Authorization = null;
        var resp = await Client.DeleteAsync($"/api/poll/delete/{Guid.NewGuid()}");
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Delete_ShouldReturn404_WhenPollNotFound()
    {
        await AuthenticateAs();
        var resp = await Client.DeleteAsync($"/api/poll/delete/{Guid.NewGuid()}");
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Vote_ShouldReturn401_WhenNotAuthenticated()
    {
        Client.DefaultRequestHeaders.Authorization = null;
        var resp = await Client.PostAsync(
            $"/api/poll/vote/{Guid.NewGuid()}/{Guid.NewGuid()}/{Guid.NewGuid()}", null);
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Vote_ShouldReturn200_WhenValid()
    {
        var (_, poll, option) = await SeedPollData();
        await AuthenticateAs();
        var voterId = Guid.NewGuid();

        var resp = await Client.PostAsync(
            $"/api/poll/vote/{poll.Id}/{option.Id}/{voterId}", null);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}

// ─────────────────────────────────────────────────────────────
// Championship Controller Tests
// ─────────────────────────────────────────────────────────────

public class ChampionshipControllerTests(TestWebAppFactory factory) : ControllerTestBase(factory)
{
    [Fact]
    public async Task GetAllChampionshipsBySeries_ShouldReturn200()
    {
        var resp = await Client.GetAsync($"/api/championship/getAllChampionshipsBySeries/{Guid.NewGuid()}");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetSeasonsBySeries_ShouldReturn200()
    {
        var resp = await Client.GetAsync($"/api/championship/getSeasonsBySeries/{Guid.NewGuid()}");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateChampionship_ShouldReturn401_WhenNotAuthenticated()
    {
        Client.DefaultRequestHeaders.Authorization = null;
        var resp = await Client.PostAsJsonAsync("/api/championship/createChampionship", new { });
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AddParticipations_ShouldReturn401_WhenNotAuthenticated()
    {
        Client.DefaultRequestHeaders.Authorization = null;
        var resp = await Client.PostAsJsonAsync("/api/championship/addParticipations", new { });
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RemoveDriverParticipation_ShouldReturn401_WhenNotAuthenticated()
    {
        Client.DefaultRequestHeaders.Authorization = null;
        var resp = await Client.DeleteAsync(
            $"/api/championship/removeDriverParticipation/{Guid.NewGuid()}/{Guid.NewGuid()}");
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RemoveConstructorCompetition_ShouldReturn401_WhenNotAuthenticated()
    {
        Client.DefaultRequestHeaders.Authorization = null;
        var resp = await Client.DeleteAsync(
            $"/api/championship/removeConstructorCompetition/{Guid.NewGuid()}/{Guid.NewGuid()}");
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}

// ─────────────────────────────────────────────────────────────
// Articles Controller Tests
// ─────────────────────────────────────────────────────────────

public class ArticlesControllerTests(TestWebAppFactory factory) : ControllerTestBase(factory)
{
    [Fact]
    public async Task GetBySlug_ShouldReturn400_WhenNotFound()
    {
        var resp = await Client.GetAsync("/api/article/get/non-existent-slug");
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetAllArticles_ShouldReturn200()
    {
        var resp = await Client.GetAsync("/api/article/getAllArticles?page=1&pageSize=10");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAllSummary_ShouldReturn200()
    {
        var resp = await Client.GetAsync("/api/article/getAllSummary?page=1&pageSize=10");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetRecent_ShouldReturn200()
    {
        var resp = await Client.GetAsync("/api/article/getRecent/5");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_ShouldReturn401_WhenNotAuthenticated()
    {
        Client.DefaultRequestHeaders.Authorization = null;
        var resp = await Client.PostAsJsonAsync("/api/article/create", new { });
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Delete_ShouldReturn401_WhenNotAuthenticated()
    {
        Client.DefaultRequestHeaders.Authorization = null;
        var resp = await Client.DeleteAsync($"/api/article/delete/{Guid.NewGuid()}");
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Delete_ShouldReturn400_WhenArticleNotFound()
    {
        await AuthenticateAs("Admin");
        var resp = await Client.DeleteAsync($"/api/article/delete/{Guid.NewGuid()}");
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}

