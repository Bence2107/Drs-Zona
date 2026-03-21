using Context;
using DTOs.Standings;
using Entities.Models.Standings;
using FluentAssertions;
using Repositories.Implementations.Standings;
using Services.Implementations;
using Services.Interfaces;
using Xunit;

namespace Tests.Services.Integrations;

public class DriverServiceIntegrationTests
{

    private static (EfContext ctx, IDriverService svc) BuildSut()
    {
        var ctx = InMemoryDbFactory.CreateContext();
        var svc = new DriverService(
            new DriversRepository(ctx),
            new ContractsRepository(ctx)
        );
        return (ctx, svc);
    }

    // ─────────────────────────────────────────────
    // GetDriverById
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetDriverById_ShouldFail_WhenNotFound()
    {
        var (_, svc) = BuildSut();

        var result = await svc.GetDriverById(Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Driver not found.");
    }

    [Fact]
    public async Task GetDriverById_ShouldReturnDriver_WithAllFields()
    {
        var (ctx, svc) = BuildSut();

        var driver = CreateDriver("Max Verstappen", "Dutch", new DateTime(1997, 9, 30));
        await ctx.Drivers.AddAsync(driver);
        await ctx.SaveChangesAsync();

        var result = await svc.GetDriverById(driver.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(driver.Id);
        result.Value.Name.Should().Be("Max Verstappen");
        result.Value.Nationality.Should().Be("Dutch");
        result.Value.TotalRaces.Should().Be(driver.TotalRaces);
        result.Value.TotalWins.Should().Be(driver.Wins);
        result.Value.TotalPodiums.Should().Be(driver.Podiums);
    }

    [Fact]
    public async Task GetDriverById_ShouldReturnContractIds_WhenContractsExist()
    {
        var (ctx, svc) = BuildSut();

        var driver      = CreateDriver();
        var constructor = CreateConstructor();
        await ctx.Drivers.AddAsync(driver);
        await ctx.Constructors.AddAsync(constructor);
        await ctx.SaveChangesAsync();

        var contract = new Contract { Id = Guid.NewGuid(), DriverId = driver.Id, ConstructorId = constructor.Id };
        await ctx.Contracts.AddAsync(contract);
        await ctx.SaveChangesAsync();

        var result = await svc.GetDriverById(driver.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value!.ConstructorIds.Should().HaveCount(1);
        result.Value.ConstructorIds!.First().Should().Be(contract.Id);
    }

    [Fact]
    public async Task GetDriverById_ShouldCalculateAge_Correctly()
    {
        var (ctx, svc) = BuildSut();

        var birthDate = DateTime.Today.AddYears(-25);
        var driver    = CreateDriver(birthDate: birthDate);
        await ctx.Drivers.AddAsync(driver);
        await ctx.SaveChangesAsync();

        var result = await svc.GetDriverById(driver.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Age.Should().Be(25);
    }

    // ─────────────────────────────────────────────
    // GetAllDrivers
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetAllDrivers_ShouldReturnAll_OrderedByName()
    {
        var (ctx, svc) = BuildSut();

        await ctx.Drivers.AddRangeAsync(
            CreateDriver("Verstappen Max"),
            CreateDriver("Alonso Fernando"),
            CreateDriver("Hamilton Lewis")
        );
        await ctx.SaveChangesAsync();

        var result = await svc.GetAllDrivers();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);
        result.Value![0].Name.Should().Be("Alonso Fernando");
        result.Value[1].Name.Should().Be("Hamilton Lewis");
        result.Value[2].Name.Should().Be("Verstappen Max");
    }

    [Fact]
    public async Task GetAllDrivers_ShouldReturnEmpty_WhenNoneExist()
    {
        var (_, svc) = BuildSut();

        var result = await svc.GetAllDrivers();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
    

    // ─────────────────────────────────────────────
    // Create
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Create_ShouldSucceed_AndPersistDriver()
    {
        var (ctx, svc) = BuildSut();

        var dto = new DriverCreateDto(
            "Lewis Hamilton", "British",
            new DateTime(1985, 1, 7),
            300, 103, 191, 7, 104, 17
        );

        var result = await svc.Create(dto);

        result.IsSuccess.Should().BeTrue();
        ctx.ChangeTracker.Clear();
        ctx.Drivers.Should().HaveCount(1);
        ctx.Drivers.First().Name.Should().Be("Lewis Hamilton");
        ctx.Drivers.First().Wins.Should().Be(103);
    }

    [Fact]
    public async Task Create_ShouldFail_WhenDriverYoungerThan15()
    {
        var (_, svc) = BuildSut();

        var dto = new DriverCreateDto(
            "Young Driver", "HU",
            DateTime.Today.AddYears(-14),
            0, 0, 0, 0, 0, 1
        );

        var result = await svc.Create(dto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Driver cannot be younger than 15 year");
    }

    [Fact]
    public async Task Create_ShouldFail_WhenBirthDateInFuture()
    {
        var (_, svc) = BuildSut();

        var dto = new DriverCreateDto(
            "Future Driver", "HU",
            DateTime.Now.AddDays(1),
            0, 0, 0, 0, 0, 1
        );

        var result = await svc.Create(dto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Birth date cannot be in the future.");
    }

    // ─────────────────────────────────────────────
    // Update
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Update_ShouldSucceed_AndPersistChanges()
    {
        var (ctx, svc) = BuildSut();

        var driver = CreateDriver("Original Name");
        await ctx.Drivers.AddAsync(driver);
        await ctx.SaveChangesAsync();

        var dto = new DriverUpdateDto(
            driver.Id,
            "Updated Name", "British",
            new DateTime(1985, 1, 7),
            300, 103, 191, 7, 104, 17
        );

        var result = await svc.Update(dto);

        result.IsSuccess.Should().BeTrue();
        ctx.ChangeTracker.Clear();
        var updated = await ctx.Drivers.FindAsync(driver.Id);
        updated!.Name.Should().Be("Updated Name");
        updated.Nationality.Should().Be("British");
        updated.Wins.Should().Be(103);
        updated.Podiums.Should().Be(191);
        updated.Championships.Should().Be(7);
    }

    [Fact]
    public async Task Update_ShouldFail_WhenDriverNotFound()
    {
        var (_, svc) = BuildSut();

        var dto = new DriverUpdateDto(
            Guid.NewGuid(),
            "Name", "HU",
            new DateTime(1990, 1, 1),
            100, 10, 30, 1, 10, 5
        );

        var result = await svc.Update(dto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Driver not found.");
    }

    [Fact]
    public async Task Update_ShouldFail_WhenBirthDateYoungerThan15()
    {
        var (ctx, svc) = BuildSut();

        var driver = CreateDriver();
        await ctx.Drivers.AddAsync(driver);
        await ctx.SaveChangesAsync();

        var dto = new DriverUpdateDto(
            driver.Id,
            "Name", "HU",
            DateTime.Today.AddYears(-14),
            0, 0, 0, 0, 0, 1
        );

        var result = await svc.Update(dto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Driver cannot be younger than 15 year");
    }

    // ─────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────

    private static Driver CreateDriver(
        string    name      = "Max Verstappen",
        string    nat       = "Dutch",
        DateTime? birthDate = null) => new()
    {
        Id            = Guid.NewGuid(),
        Name          = name,
        Nationality   = nat,
        BirthDate     = birthDate ?? new DateTime(1997, 9, 30),
        TotalRaces    = 180,
        Wins          = 55,
        Podiums       = 95,
        Championships = 3,
        PolePositions = 30,
        Seasons       = 10
    };

    private static Constructor CreateConstructor(string name = "Red Bull") => new()
    {
        Id = Guid.NewGuid(),
        Name = name,
        Nickname = name.Length >= 3
            ? name[..3]
                .ToUpper()
            : "TM",
        HeadQuarters = "null",
        TeamChief = "null",
        TechnicalChief = "null"
    };
}
