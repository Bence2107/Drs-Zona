using DTOs.Standings;
using Entities.Models.RaceTracks;
using Entities.Models.Standings;
using FluentAssertions;
using Moq;
using Repositories.Interfaces.RaceTracks;
using Repositories.Interfaces.Standings;
using Services.Implementations;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Services.Units;

public class ChampionshipServiceUnitTests
{
    private readonly ITestOutputHelper _output;

    private readonly Mock<IConstructorsRepository>              _constructorsRepo            = new();
    private readonly Mock<IConstructorsChampionshipsRepository> _constructorChampRepo        = new();
    private readonly Mock<IConstructorCompetitionRepository>    _constructorCompetitionRepo  = new();
    private readonly Mock<IContractsRepository>                 _contractsRepo               = new();
    private readonly Mock<IDriversRepository>                   _driversRepo                 = new();
    private readonly Mock<IDriversChampionshipsRepository>      _driverChampRepo             = new();
    private readonly Mock<IDriverParticipationRepository>       _driverParticipationRepo     = new();
    private readonly Mock<IGrandsPrixRepository>                _grandsPrixRepo              = new();
    private readonly Mock<IResultsRepository>                   _resultsRepo                 = new();

    private readonly ChampionshipService _svc;

    public ChampionshipServiceUnitTests(ITestOutputHelper output)
    {
        _output = output;
        _svc = new ChampionshipService(
            _constructorsRepo.Object,
            _constructorChampRepo.Object,
            _constructorCompetitionRepo.Object,
            _contractsRepo.Object,
            _driversRepo.Object,
            _driverChampRepo.Object,
            _driverParticipationRepo.Object,
            _grandsPrixRepo.Object,
            _resultsRepo.Object
        );
    }

    // ─────────────────────────────────────────────
    // GetAllChampionshipsBySeries
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetAllChampionshipsBySeries_ShouldReturnJoinedRows_OrderedBySeasonDescending()
    {
        var seriesId = Guid.NewGuid();
        var series   = CreateSeries(seriesId);

        var driverChamps = new List<DriversChampionship>
        {
            CreateDriversChamp(seriesId, "2023", "WDC 2023", series),
            CreateDriversChamp(seriesId, "2024", "WDC 2024", series)
        };
        var constChamps = new List<ConstructorsChampionship>
        {
            CreateConstructorsChamp(seriesId, "2023", "WCC 2023"),
            CreateConstructorsChamp(seriesId, "2024", "WCC 2024")
        };

        _driverChampRepo.Setup(r => r.GetBySeriesId(seriesId)).ReturnsAsync(driverChamps);
        _constructorChampRepo.Setup(r => r.GetBySeriesId(seriesId)).ReturnsAsync(constChamps);

        var result = await _svc.GetAllChampionshipsBySeries(seriesId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value![0].Season.Should().Be("2024");
        result.Value[1].Season.Should().Be("2023");
    }

    [Fact]
    public async Task GetAllChampionshipsBySeries_ShouldReturnEmpty_WhenNoMatchingSeason()
    {
        var seriesId = Guid.NewGuid();

        _driverChampRepo.Setup(r => r.GetBySeriesId(seriesId))
            .ReturnsAsync(new List<DriversChampionship>());
        _constructorChampRepo.Setup(r => r.GetBySeriesId(seriesId))
            .ReturnsAsync(new List<ConstructorsChampionship>());

        var result = await _svc.GetAllChampionshipsBySeries(seriesId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    // ─────────────────────────────────────────────
    // GetSeasonsBySeries
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetSeasonsBySeries_ShouldReturnYearLookups_OrderedDescending()
    {
        var seriesId = Guid.NewGuid();

        _driverChampRepo.Setup(r => r.GetBySeriesId(seriesId)).ReturnsAsync(new List<DriversChampionship>
        {
            CreateDriversChamp(seriesId, "2022"),
            CreateDriversChamp(seriesId, "2024"),
        });
        _constructorChampRepo.Setup(r => r.GetBySeriesId(seriesId)).ReturnsAsync(new List<ConstructorsChampionship>
        {
            CreateConstructorsChamp(seriesId, "2022"),
            CreateConstructorsChamp(seriesId, "2024"),
        });

        var result = await _svc.GetSeasonsBySeries(seriesId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value![0].Season.Should().Be("2024");
        result.Value[1].Season.Should().Be("2022");
    }

    // ─────────────────────────────────────────────
    // GetGrandsPrixByChampionship
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetGrandsPrixByChampionship_ShouldFail_WhenChampionshipNotFound()
    {
        var champId = Guid.NewGuid();
        _driverChampRepo.Setup(r => r.GetById(champId)).ReturnsAsync((DriversChampionship?)null);

        var result = await _svc.GetGrandsPrixByChampionship(champId);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Bajnokság nem található");
    }

    [Fact]
    public async Task GetGrandsPrixByChampionship_ShouldReturnGrandsPrix_OrderedByRoundNumber()
    {
        var seriesId = Guid.NewGuid();
        var champId  = Guid.NewGuid();
        var champ    = CreateDriversChamp(seriesId, "2024", id: champId);

        var circuit = new Circuit
        {
            Id = Guid.NewGuid(),
            Name = "Monza",
            Location = "Italy",
            Type = "Pernament",
            FastestLap = "null"
        };
        var gps = new List<GrandPrix>
        {
            CreateGrandPrix(seriesId, 2024, 2, circuit),
            CreateGrandPrix(seriesId, 2024, 1, circuit),
        };

        _driverChampRepo.Setup(r => r.GetById(champId)).ReturnsAsync(champ);
        _grandsPrixRepo.Setup(r => r.GetBySeriesAndYear(seriesId, 2024)).ReturnsAsync(gps);
        _resultsRepo.Setup(r => r.HasGrandPrixResults(It.IsAny<GrandPrix>())).ReturnsAsync(false);

        var result = await _svc.GetGrandsPrixByChampionship(champId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value![0].RoundNumber.Should().Be(1);
        result.Value[1].RoundNumber.Should().Be(2);
    }

    // ─────────────────────────────────────────────
    // CreateChampionship
    // ─────────────────────────────────────────────

    [Fact]
    public async Task CreateChampionship_ShouldSucceed_AndCallBothRepos()
    {
        var seriesId = Guid.NewGuid();

        _driverChampRepo.Setup(r => r.Create(It.IsAny<DriversChampionship>())).Returns(Task.CompletedTask);
        _constructorChampRepo.Setup(r => r.Create(It.IsAny<ConstructorsChampionship>())).Returns(Task.CompletedTask);

        var dto = new ChampionshipCreateDto(seriesId, "2024", "WDC 2024", "WCC 2024");

        var result = await _svc.CreateChampionship(dto);

        result.IsSuccess.Should().BeTrue();
        _driverChampRepo.Verify(r => r.Create(It.Is<DriversChampionship>(d =>
            d.Season == "2024" && d.Name == "WDC 2024" && d.Status == "Upcoming"
        )), Times.Once);
        _constructorChampRepo.Verify(r => r.Create(It.Is<ConstructorsChampionship>(c =>
            c.Season == "2024" && c.Name == "WCC 2024" && c.Status == "Upcoming"
        )), Times.Once);
    }

    // ─────────────────────────────────────────────
    // UpdateChampionshipStatus
    // ─────────────────────────────────────────────

    [Fact]
    public async Task UpdateChampionshipStatus_ShouldFail_WhenDriversChampNotFound()
    {
        _driverChampRepo.Setup(r => r.GetById(It.IsAny<Guid>())).ReturnsAsync((DriversChampionship?)null);

        var result = await _svc.UpdateChampionshipStatus(Guid.NewGuid(), Guid.NewGuid(), "Active");

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Egyéni bajnokság nem található");
    }

    [Fact]
    public async Task UpdateChampionshipStatus_ShouldFail_WhenConstructorsChampNotFound()
    {
        var driverChamp = CreateDriversChamp(Guid.NewGuid(), "2024");
        _driverChampRepo.Setup(r => r.GetById(driverChamp.Id)).ReturnsAsync(driverChamp);
        _constructorChampRepo.Setup(r => r.GetByIdWithSeries(It.IsAny<Guid>())).ReturnsAsync((ConstructorsChampionship?)null);

        var result = await _svc.UpdateChampionshipStatus(driverChamp.Id, Guid.NewGuid(), "Active");

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Konstruktőri bajnokság nem található");
    }

    [Fact]
    public async Task UpdateChampionshipStatus_ShouldSucceed_AndUpdateBothStatuses()
    {
        var driverChamp     = CreateDriversChamp(Guid.NewGuid(), "2024");
        var constructorChamp = CreateConstructorsChamp(Guid.NewGuid(), "2024");

        _driverChampRepo.Setup(r => r.GetById(driverChamp.Id)).ReturnsAsync(driverChamp);
        _constructorChampRepo.Setup(r => r.GetByIdWithSeries(constructorChamp.Id)).ReturnsAsync(constructorChamp);
        _driverChampRepo.Setup(r => r.Modify(It.IsAny<DriversChampionship>())).Returns(Task.CompletedTask);
        _constructorChampRepo.Setup(r => r.Update(It.IsAny<ConstructorsChampionship>())).Returns(Task.CompletedTask);

        var result = await _svc.UpdateChampionshipStatus(driverChamp.Id, constructorChamp.Id, "Finished");

        result.IsSuccess.Should().BeTrue();
        driverChamp.Status.Should().Be("Finished");
        constructorChamp.Status.Should().Be("Finished");
    }

    // ─────────────────────────────────────────────
    // CreateParticipations
    // ─────────────────────────────────────────────

    [Fact]
    public async Task CreateParticipations_ShouldFail_WhenConstructorNotFound()
    {
        var constructorId    = Guid.NewGuid();
        var constChampId     = Guid.NewGuid();
        var driversChampId   = Guid.NewGuid();

        _constructorCompetitionRepo.Setup(r => r.CheckIfExists(constructorId, constChampId)).ReturnsAsync(false);
        _constructorsRepo.Setup(r => r.GetByIdWithBrand(constructorId)).ReturnsAsync((Constructor?)null);

        var dto = new ParticipationAddDto(driversChampId, constChampId, new List<DriverParticipationRowDto>(), new List<Guid> { constructorId });

        var result = await _svc.CreateParticipations(dto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Constructor dosen't exist");
    }

    [Fact]
    public async Task CreateParticipations_ShouldFail_WhenDriverNotFound()
    {
        var driverId       = Guid.NewGuid();
        var driversChampId = Guid.NewGuid();
        var constChampId   = Guid.NewGuid();

        _driverParticipationRepo.Setup(r => r.CheckIfExists(driverId, driversChampId)).ReturnsAsync(false);
        _driversRepo.Setup(r => r.GetDriverById(driverId)).ReturnsAsync((Driver?)null);

        var driver = new TestDriverParticipationRowDto(driverId, 44);
        var dto    = new ParticipationAddDto(driversChampId, constChampId, new List<DriverParticipationRowDto> { driver }, new List<Guid>());

        var result = await _svc.CreateParticipations(dto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Driver dosen't exist");
    }

    [Fact]
    public async Task CreateParticipations_ShouldSkip_WhenAlreadyExists()
    {
        var constructorId  = Guid.NewGuid();
        var constChampId   = Guid.NewGuid();
        var driversChampId = Guid.NewGuid();

        // Már létezik — skip
        _constructorCompetitionRepo.Setup(r => r.CheckIfExists(constructorId, constChampId)).ReturnsAsync(true);

        var dto = new ParticipationAddDto(driversChampId, constChampId, new List<DriverParticipationRowDto>(), new List<Guid> { constructorId });

        var result = await _svc.CreateParticipations(dto);

        result.IsSuccess.Should().BeTrue();
        _constructorCompetitionRepo.Verify(r => r.Create(It.IsAny<ConstructorCompetition>()), Times.Never);
    }

    // ─────────────────────────────────────────────
    // DeleteDriverParticipation / DeleteConstructorCompetition
    // ─────────────────────────────────────────────

    [Fact]
    public async Task DeleteDriverParticipation_ShouldSucceed_AndCallDelete()
    {
        var driverId   = Guid.NewGuid();
        var champId    = Guid.NewGuid();

        _driverParticipationRepo.Setup(r => r.Delete(driverId, champId)).Returns(Task.CompletedTask);

        var result = await _svc.DeleteDriverParticipation(driverId, champId);

        result.IsSuccess.Should().BeTrue();
        _driverParticipationRepo.Verify(r => r.Delete(driverId, champId), Times.Once);
    }

    [Fact]
    public async Task DeleteConstructorCompetition_ShouldSucceed_AndCallDelete()
    {
        var constructorId = Guid.NewGuid();
        var champId       = Guid.NewGuid();

        _constructorCompetitionRepo.Setup(r => r.Delete(constructorId, champId)).Returns(Task.CompletedTask);

        var result = await _svc.DeleteConstructorCompetition(constructorId, champId);

        result.IsSuccess.Should().BeTrue();
        _constructorCompetitionRepo.Verify(r => r.Delete(constructorId, champId), Times.Once);
    }

    // ─────────────────────────────────────────────
    // GetDriversBySeason
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetDriversBySeason_ShouldReturnDrivers_OrderedByName()
    {
        var champId = Guid.NewGuid();
        var driver1Id = Guid.NewGuid();
        var driver2Id = Guid.NewGuid();

        var participations = new List<DriverParticipation>
        {
            new() { DriverId = driver1Id, DriverNameSnapshot = "Verstappen Max",  DriverChampId = champId },
            new() { DriverId = driver2Id, DriverNameSnapshot = "Hamilton Lewis",   DriverChampId = champId },
        };

        _driverParticipationRepo.Setup(r => r.GetByChampionshipId(champId)).ReturnsAsync(participations);
        _contractsRepo.Setup(r => r.GetByDriverId(It.IsAny<Guid>())).ReturnsAsync(new List<Contract>());

        var result = await _svc.GetDriversBySeason(champId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value![0].Name.Should().Be("Hamilton Lewis");
        result.Value[1].Name.Should().Be("Verstappen Max");
    }

    // ─────────────────────────────────────────────
    // GetConstructorsBySeason
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetConstructorsBySeason_ShouldReturnConstructors_OrderedByName()
    {
        var champId = Guid.NewGuid();
        var constructor1 = CreateConstructor("Red Bull");
        var constructor2 = CreateConstructor("Mercedes");

        var competitions = new List<ConstructorCompetition>
        {
            new() { ConstructorId = constructor1.Id, Constructor = constructor1, ConstructorNameSnapshot = "Red Bull",  ConstChampId = champId, ConstructorNicknameSnapshot = "RBR" },
            new() { ConstructorId = constructor2.Id, Constructor = constructor2, ConstructorNameSnapshot = "Mercedes", ConstChampId = champId, ConstructorNicknameSnapshot = "MER" },
        };

        _constructorCompetitionRepo.Setup(r => r.GetByChampionshipId(champId)).ReturnsAsync(competitions);

        var result = await _svc.GetConstructorsBySeason(champId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value![0].Name.Should().Be("Mercedes");
        result.Value[1].Name.Should().Be("Red Bull");
    }

    // ─────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────

    private static Series CreateSeries(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        Name = "Formula 1",
        ShortName = "F1",
        Description = "null",
        GoverningBody = "null",
        PointSystem = "null"
    };

    private static DriversChampionship CreateDriversChamp(
        Guid seriesId, string season, string name = "WDC", Series? series = null, Guid? id = null) => new()
    {
        Id       = id ?? Guid.NewGuid(),
        SeriesId = seriesId,
        Season   = season,
        Name     = name,
        Status   = "Upcoming",
        Series   = series
    };

    private static ConstructorsChampionship CreateConstructorsChamp(
        Guid seriesId, string season, string name = "WCC", Guid? id = null) => new()
    {
        Id       = id ?? Guid.NewGuid(),
        SeriesId = seriesId,
        Season   = season,
        Name     = name,
        Status   = "Upcoming"
    };

    private static Constructor CreateConstructor(string name) => new()
    {
        Id = Guid.NewGuid(),
        Name = name,
        Nickname = name[..3]
            .ToUpper(),
        HeadQuarters = "null",
        TeamChief = "null",
        TechnicalChief = "null"
    };

    private static GrandPrix CreateGrandPrix(Guid seriesId, int year, int round, Circuit circuit) => new()
    {
        Id           = Guid.NewGuid(),
        SeriesId     = seriesId,
        SeasonYear   = year,
        RoundNumber  = round,
        Name         = $"GP Round {round}",
        ShortName    = $"R{round}",
        Circuit      = circuit,
        CircuitId    = circuit.Id,
        StartTime    = DateTime.UtcNow,
        EndTime      = DateTime.UtcNow.AddHours(2),
        RaceDistance = 305,
        LapsCompleted = 58
    };

    // DriverParticipationRowDto abstract — konkrét implementáció a teszthez
    private sealed record TestDriverParticipationRowDto(Guid DriverId, int DriverNumber)
        : DriverParticipationRowDto(DriverId, DriverNumber);
}
