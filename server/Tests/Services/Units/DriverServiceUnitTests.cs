using DTOs.Standings;
using Entities.Models.Standings;
using FluentAssertions;
using Moq;
using Repositories.Interfaces.Standings;
using Services.Implementations;
using Xunit;

namespace Tests.Services.Units;

public class DriverServiceUnitTests
{

    private readonly Mock<IDriversRepository>    _driverRepo    = new();
    private readonly Mock<IContractsRepository>  _contractsRepo = new();

    private readonly DriverService _svc;

    public DriverServiceUnitTests()
    {
        _svc = new DriverService(_driverRepo.Object, _contractsRepo.Object);
    }

    // ─────────────────────────────────────────────
    // GetDriverById
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetDriverById_ShouldFail_WhenNotFound()
    {
        var id = Guid.NewGuid();
        _driverRepo.Setup(r => r.GetDriverById(id)).ReturnsAsync((Driver?)null);

        var result = await _svc.GetDriverById(id);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Driver not found.");
    }

    [Fact]
    public async Task GetDriverById_ShouldReturnDto_WithCorrectFields()
    {
        var driver = CreateDriver();
        _driverRepo.Setup(r => r.GetDriverById(driver.Id)).ReturnsAsync(driver);
        _contractsRepo.Setup(r => r.GetByDriverId(driver.Id)).ReturnsAsync(new List<Contract>());

        var result = await _svc.GetDriverById(driver.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(driver.Id);
        result.Value.Name.Should().Be(driver.Name);
        result.Value.Nationality.Should().Be(driver.Nationality);
        result.Value.TotalRaces.Should().Be(driver.TotalRaces);
        result.Value.TotalWins.Should().Be(driver.Wins);
        result.Value.TotalPodiums.Should().Be(driver.Podiums);
        result.Value.Championships.Should().Be(driver.Championships);
        result.Value.PolePositions.Should().Be(driver.PolePositions);
        result.Value.Seasons.Should().Be(driver.Seasons);
    }

    [Fact]
    public async Task GetDriverById_ShouldReturnContractIds_WhenContractsExist()
    {
        var driver = CreateDriver();
        var c1 = new Contract { Id = Guid.NewGuid(), DriverId = driver.Id };
        var c2 = new Contract { Id = Guid.NewGuid(), DriverId = driver.Id };

        _driverRepo.Setup(r => r.GetDriverById(driver.Id)).ReturnsAsync(driver);
        _contractsRepo.Setup(r => r.GetByDriverId(driver.Id)).ReturnsAsync([c1, c2]);

        var result = await _svc.GetDriverById(driver.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value!.ConstructorIds.Should().HaveCount(2);
        result.Value.ConstructorIds.Should().Contain(c1.Id);
        result.Value.ConstructorIds.Should().Contain(c2.Id);
    }

    [Fact]
    public async Task GetDriverById_ShouldCalculateAge_Correctly()
    {
        var birthDate = DateTime.Today.AddYears(-25);
        var driver    = CreateDriver(birthDate: birthDate);

        _driverRepo.Setup(r => r.GetDriverById(driver.Id)).ReturnsAsync(driver);
        _contractsRepo.Setup(r => r.GetByDriverId(driver.Id)).ReturnsAsync(new List<Contract>());

        var result = await _svc.GetDriverById(driver.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Age.Should().Be(25);
    }

    // ─────────────────────────────────────────────
    // GetAllDrivers
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetAllDrivers_ShouldReturnList_OrderedByName()
    {
        var drivers = new List<Driver>
        {
            CreateDriver(name: "Verstappen Max"),
            CreateDriver(name: "Alonso Fernando"),
            CreateDriver(name: "Hamilton Lewis"),
        };

        _driverRepo.Setup(r => r.GetAllDrivers()).ReturnsAsync(drivers);
        _contractsRepo.Setup(r => r.GetByDriverId(It.IsAny<Guid>())).ReturnsAsync(new List<Contract>());

        var result = await _svc.GetAllDrivers();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);
        result.Value![0].Name.Should().Be("Alonso Fernando");
        result.Value[1].Name.Should().Be("Hamilton Lewis");
        result.Value[2].Name.Should().Be("Verstappen Max");
    }

    [Fact]
    public async Task GetAllDrivers_ShouldReturnEmpty_WhenNoDriversExist()
    {
        _driverRepo.Setup(r => r.GetAllDrivers()).ReturnsAsync(new List<Driver>());

        var result = await _svc.GetAllDrivers();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllDrivers_ShouldReturnCurrentTeamName_FromLastContract()
    {
        var driver     = CreateDriver();
        var constructor = new Constructor
        {
            Id = Guid.NewGuid(),
            Name = "Red Bull",
            Nickname = "null",
            HeadQuarters = "null",
            TeamChief = "null",
            TechnicalChief = "null"
        };
        var contracts  = new List<Contract>
        {
            new() { Id = Guid.NewGuid(), DriverId = driver.Id, Constructor = new Constructor
                {
                    Name = "Williams",
                    Nickname = "null",
                    HeadQuarters = "null",
                    TeamChief = "null",
                    TechnicalChief = "null"
                }
            },
            new() { Id = Guid.NewGuid(), DriverId = driver.Id, Constructor = constructor },
        };

        _driverRepo.Setup(r => r.GetAllDrivers()).ReturnsAsync([driver]);
        _contractsRepo.Setup(r => r.GetByDriverId(driver.Id)).ReturnsAsync(contracts);

        var result = await _svc.GetAllDrivers();

        result.IsSuccess.Should().BeTrue();
        result.Value![0].CurrentTeam.Should().Be("Red Bull");
    }

    [Fact]
    public async Task GetAllDrivers_ShouldReturnNullTeam_WhenNoContracts()
    {
        var driver = CreateDriver();

        _driverRepo.Setup(r => r.GetAllDrivers()).ReturnsAsync([driver]);
        _contractsRepo.Setup(r => r.GetByDriverId(driver.Id)).ReturnsAsync([]);

        var result = await _svc.GetAllDrivers();

        result.IsSuccess.Should().BeTrue();
        result.Value![0].CurrentTeam.Should().BeNull();
    }

    // ─────────────────────────────────────────────
    // Create
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Create_ShouldFail_WhenDriverYoungerThan15()
    {
        var dto = CreateDriverCreateDto(birthDate: DateTime.Today.AddYears(-14));

        var result = await _svc.Create(dto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Driver cannot be younger than 15 year");
    }

    [Fact]
    public async Task Create_ShouldFail_WhenBirthDateInFuture()
    {
        var dto = CreateDriverCreateDto(birthDate: DateTime.Now.AddDays(1));

        var result = await _svc.Create(dto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Birth date cannot be in the future.");
    }

    [Fact]
    public async Task Create_ShouldSucceed_AndCallRepoCreate()
    {
        _driverRepo.Setup(r => r.Create(It.IsAny<Driver>())).Returns(Task.CompletedTask);

        var dto = CreateDriverCreateDto();

        var result = await _svc.Create(dto);

        result.IsSuccess.Should().BeTrue();
        _driverRepo.Verify(r => r.Create(It.Is<Driver>(d =>
            d.Name        == dto.Name        &&
            d.Nationality == dto.Nationality
        )), Times.Once);
    }

    [Fact]
    public async Task Create_ShouldPersistAllFields()
    {
        Driver? saved = null;
        _driverRepo
            .Setup(r => r.Create(It.IsAny<Driver>()))
            .Callback<Driver>(d => saved = d)
            .Returns(Task.CompletedTask);

        var dto = CreateDriverCreateDto(name: "Lewis Hamilton", nationality: "British");

        await _svc.Create(dto);

        saved.Should().NotBeNull();
        saved!.Name.Should().Be("Lewis Hamilton");
        saved.Nationality.Should().Be("British");
        saved.TotalRaces.Should().Be(dto.TotalRaces);
        saved.Wins.Should().Be(dto.TotalWins);
        saved.Podiums.Should().Be(dto.TotalPodiums);
        saved.Championships.Should().Be(dto.Championships);
        saved.PolePositions.Should().Be(dto.PolePositions);
        saved.Seasons.Should().Be(dto.Seasons);
    }

    // ─────────────────────────────────────────────
    // Update
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Update_ShouldFail_WhenDriverNotFound()
    {
        _driverRepo.Setup(r => r.GetDriverById(It.IsAny<Guid>())).ReturnsAsync((Driver?)null);

        var dto = CreateDriverUpdateDto();

        var result = await _svc.Update(dto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Driver not found.");
    }

    [Fact]
    public async Task Update_ShouldFail_WhenBirthDateYoungerThan15()
    {
        var driver = CreateDriver();
        _driverRepo.Setup(r => r.GetDriverById(driver.Id)).ReturnsAsync(driver);

        var dto = CreateDriverUpdateDto(id: driver.Id, birthDate: DateTime.Today.AddYears(-14));

        var result = await _svc.Update(dto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Driver cannot be younger than 15 year");
    }

    [Fact]
    public async Task Update_ShouldFail_WhenBirthDateInFuture()
    {
        var driver = CreateDriver();
        _driverRepo.Setup(r => r.GetDriverById(driver.Id)).ReturnsAsync(driver);

        var dto = CreateDriverUpdateDto(id: driver.Id, birthDate: DateTime.UtcNow.AddDays(1));

        var result = await _svc.Update(dto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Birth date cannot be in the future.");
    }

    [Fact]
    public async Task Update_ShouldSucceed_AndCallRepoUpdate()
    {
        var driver = CreateDriver();
        _driverRepo.Setup(r => r.GetDriverById(driver.Id)).ReturnsAsync(driver);
        _driverRepo.Setup(r => r.Update(It.IsAny<Driver>())).Returns(Task.CompletedTask);

        var dto = CreateDriverUpdateDto(id: driver.Id, name: "Updated Name");

        var result = await _svc.Update(dto);

        result.IsSuccess.Should().BeTrue();
        _driverRepo.Verify(r => r.Update(It.Is<Driver>(d => d.Name == "Updated Name")), Times.Once);
    }

    [Fact]
    public async Task Update_ShouldUpdateAllFields()
    {
        var driver = CreateDriver();
        Driver? updated = null;

        _driverRepo.Setup(r => r.GetDriverById(driver.Id)).ReturnsAsync(driver);
        _driverRepo
            .Setup(r => r.Update(It.IsAny<Driver>()))
            .Callback<Driver>(d => updated = d)
            .Returns(Task.CompletedTask);

        var dto = CreateDriverUpdateDto(
            id:          driver.Id,
            name:        "New Name",
            nationality: "Dutch",
            totalRaces:  300,
            totalWins:   50,
            podiums:     100,
            champs:      4,
            poles:       40,
            seasons:     15
        );

        await _svc.Update(dto);

        updated!.Name.Should().Be("New Name");
        updated.Nationality.Should().Be("Dutch");
        updated.TotalRaces.Should().Be(300);
        updated.Wins.Should().Be(50);
        updated.Podiums.Should().Be(100);
        updated.Championships.Should().Be(4);
        updated.PolePositions.Should().Be(40);
        updated.Seasons.Should().Be(15);
    }

    // ─────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────

    private static Driver CreateDriver(
        string?   name      = null,
        DateTime? birthDate = null) => new()
    {
        Id            = Guid.NewGuid(),
        Name          = name      ?? "Max Verstappen",
        Nationality   = "Dutch",
        BirthDate     = birthDate ?? new DateTime(1997, 9, 30),
        TotalRaces    = 180,
        Wins          = 55,
        Podiums       = 95,
        Championships = 3,
        PolePositions = 30,
        Seasons       = 10
    };

    private static DriverCreateDto CreateDriverCreateDto(
        string?   name        = null,
        string?   nationality = null,
        DateTime? birthDate   = null) => new(
        name        ?? "Max Verstappen",
        nationality ?? "Dutch",
        birthDate   ?? new DateTime(1997, 9, 30),
        180, 55, 95, 3, 30, 10
    );

    private static DriverUpdateDto CreateDriverUpdateDto(
        Guid?     id          = null,
        string?   name        = null,
        string?   nationality = null,
        DateTime? birthDate   = null,
        int       totalRaces  = 180,
        int       totalWins   = 55,
        int       podiums     = 95,
        int       champs      = 3,
        int       poles       = 30,
        int       seasons     = 10) => new(
        id          ?? Guid.NewGuid(),
        name        ?? "Max Verstappen",
        nationality ?? "Dutch",
        birthDate   ?? new DateTime(1997, 9, 30),
        totalRaces, totalWins, podiums, champs, poles, seasons
    );
}
