using DTOs.Standings;
using Entities.Models.RaceTracks;
using Entities.Models.Standings;
using FluentAssertions;
using Moq;
using Repositories.Interfaces.RaceTracks;
using Repositories.Interfaces.Standings;
using Services.Implementations;
using Xunit;

namespace Tests.Services.Units;

public class StandingsServiceUnitTests
{

    private readonly Mock<IConstructorsChampionshipsRepository> _constructorChampRepo = new();
    private readonly Mock<IConstructorCompetitionRepository> _constructorCompetitionRepo = new();
    private readonly Mock<IDriversChampionshipsRepository> _driverChampRepo = new();
    private readonly Mock<IDriverParticipationRepository> _driverParticipationRepo = new();
    private readonly Mock<IGrandsPrixRepository> _grandsPrixRepo = new();
    private readonly Mock<IQualifyingResultRepository> _qualifyingRepo = new();
    private readonly Mock<IResultsRepository> _resultsRepo = new();
    private readonly Mock<ISeriesRepository> _seriesRepo = new();

    private readonly StandingsService _svc;

    public StandingsServiceUnitTests()
    {
        _svc = new StandingsService(
            _constructorChampRepo.Object,
            _constructorCompetitionRepo.Object,
            _driverChampRepo.Object,
            _driverParticipationRepo.Object,
            _grandsPrixRepo.Object,
            _qualifyingRepo.Object,
            _resultsRepo.Object,
            _seriesRepo.Object
        );
    }

    // ─────────────────────────────────────────────
    // GetAllSeries
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetAllSeries_ShouldReturnAllSeries()
    {
        _seriesRepo.Setup(r => r.GetAllSeries()).ReturnsAsync([
            CreateSeries(),
            CreateSeries("Formula 2", "F2")
        ]);

        var result = await _svc.GetAllSeries();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value![0].Name.Should().Be("F1");
        result.Value[1].Name.Should().Be("F2");
    }

    [Fact]
    public async Task GetAllSeries_ShouldReturnEmpty_WhenNoneExist()
    {
        _seriesRepo.Setup(r => r.GetAllSeries()).ReturnsAsync(new List<Series>());

        var result = await _svc.GetAllSeries();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    // ─────────────────────────────────────────────
    // GetConstructorStandings
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetConstructorStandings_ShouldFail_WhenChampionshipNotFound()
    {
        var id = Guid.NewGuid();
        _constructorChampRepo.Setup(r => r.GetByIdWithSeries(id)).ReturnsAsync((ConstructorsChampionship?)null);

        var result = await _svc.GetConstructorStandings(id);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("A konstruktőri bajnokság nem található");
    }

    [Fact]
    public async Task GetConstructorStandings_ShouldReturnEmpty_WhenNoResults()
    {
        var champId = Guid.NewGuid();
        _constructorChampRepo.Setup(r => r.GetByIdWithSeries(champId))
            .ReturnsAsync(CreateConstructorsChamp(champId));
        _resultsRepo.Setup(r => r.GetByConstructorsChampionshipId(champId))
            .ReturnsAsync([]);

        var result = await _svc.GetConstructorStandings(champId);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Results.Should().BeEmpty();
    }

    [Fact]
    public async Task GetConstructorStandings_ShouldGroupByConstructor_AndSumPoints()
    {
        var champId = Guid.NewGuid();
        var constructorId = Guid.NewGuid();
        var gp = CreateGrandPrix(round: 1);

        _constructorChampRepo.Setup(r => r.GetByIdWithSeries(champId))
            .ReturnsAsync(CreateConstructorsChamp(champId));
        _resultsRepo.Setup(r => r.GetByConstructorsChampionshipId(champId))
            .ReturnsAsync([
                CreateResult(constructorId: constructorId, constructorPoints: 25, finishPosition: 1, gp: gp),
                CreateResult(constructorId: constructorId, constructorPoints: 18, finishPosition: 2, gp: gp)
            ]);

        var result = await _svc.GetConstructorStandings(champId);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Results.Should().HaveCount(1);
        result.Value.Results[0].Points.Should().Be(43);
    }

    // ─────────────────────────────────────────────
    // GetDriverStandings
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetDriverStandings_ShouldFail_WhenChampionshipNotFound()
    {
        var id = Guid.NewGuid();
        _driverChampRepo.Setup(r => r.GetByIdWithSeries(id)).ReturnsAsync((DriversChampionship?)null);

        var result = await _svc.GetDriverStandings(id);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Az egyéni bajnokság nem található");
    }

    [Fact]
    public async Task GetDriverStandings_ShouldFail_WhenSeriesNotFound()
    {
        var champId = Guid.NewGuid();
        var seriesId = Guid.NewGuid();
        var champ = CreateDriversChamp(champId, seriesId);

        _driverChampRepo.Setup(r => r.GetByIdWithSeries(champId)).ReturnsAsync(champ);
        _seriesRepo.Setup(r => r.GetSeriesById(seriesId)).ReturnsAsync((Series?)null);

        var result = await _svc.GetDriverStandings(champId);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("A széria nem található");
    }

    [Fact]
    public async Task GetDriverStandings_ShouldReturnEmpty_WhenNoResults()
    {
        var champId = Guid.NewGuid();
        var seriesId = Guid.NewGuid();

        _driverChampRepo.Setup(r => r.GetByIdWithSeries(champId)).ReturnsAsync(CreateDriversChamp(champId, seriesId));
        _seriesRepo.Setup(r => r.GetSeriesById(seriesId)).ReturnsAsync(CreateSeries());
        _resultsRepo.Setup(r => r.GetByDriversChampionshipId(champId)).ReturnsAsync(new List<Result>());

        var result = await _svc.GetDriverStandings(champId);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Results.Should().BeEmpty();
    }

    [Fact]
    public async Task GetDriverStandings_ShouldGroupByDriver_AndSumPoints()
    {
        var champId = Guid.NewGuid();
        var seriesId = Guid.NewGuid();
        var driverId = Guid.NewGuid();
        var gp1 = CreateGrandPrix(round: 1);
        var gp2 = CreateGrandPrix(round: 2);

        _driverChampRepo.Setup(r => r.GetByIdWithSeries(champId)).ReturnsAsync(CreateDriversChamp(champId, seriesId));
        _seriesRepo.Setup(r => r.GetSeriesById(seriesId)).ReturnsAsync(CreateSeries());
        _resultsRepo.Setup(r => r.GetByDriversChampionshipId(champId)).ReturnsAsync([
            CreateResult(driverId: driverId, driverPoints: 25, finishPosition: 1, gp: gp1),
            CreateResult(driverId: driverId, driverPoints: 18, finishPosition: 2, gp: gp2)
        ]);

        var result = await _svc.GetDriverStandings(champId);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Results.Should().HaveCount(1);
        result.Value.Results[0].Points.Should().Be(43);
    }

    [Fact]
    public async Task GetDriverStandings_ShouldOrderByPoints_Descending()
    {
        var champId = Guid.NewGuid();
        var seriesId = Guid.NewGuid();
        var driver1 = Guid.NewGuid();
        var driver2 = Guid.NewGuid();
        var gp = CreateGrandPrix(round: 1);

        _driverChampRepo.Setup(r => r.GetByIdWithSeries(champId)).ReturnsAsync(CreateDriversChamp(champId, seriesId));
        _seriesRepo.Setup(r => r.GetSeriesById(seriesId)).ReturnsAsync(CreateSeries());
        _resultsRepo.Setup(r => r.GetByDriversChampionshipId(champId)).ReturnsAsync([
            CreateResult(driverId: driver1, driverPoints: 10, driverName: "Hamilton", finishPosition: 3, gp: gp),
            CreateResult(driverId: driver2, driverPoints: 25, driverName: "Verstappen", finishPosition: 1, gp: gp)
        ]);

        var result = await _svc.GetDriverStandings(champId);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Results[0].DriverName.Should().Be("Verstappen");
        result.Value.Results[1].DriverName.Should().Be("Hamilton");
        result.Value.Results[0].Position.Should().Be(1);
    }

    // ─────────────────────────────────────────────
    // GetGrandPrixContext
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetGrandPrixContext_ShouldFail_WhenGrandPrixNotFound()
    {
        var gpId = Guid.NewGuid();
        _grandsPrixRepo.Setup(r => r.GetGrandPrixById(gpId)).ReturnsAsync((GrandPrix?)null);

        var result = await _svc.GetGrandPrixContext(gpId);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Nagydíj nem található");
    }

    [Fact]
    public async Task GetGrandPrixContext_ShouldFail_WhenSeriesNotFound()
    {
        var seriesId = Guid.NewGuid();
        var gp = new GrandPrix
        {
            Id = Guid.NewGuid(),
            SeriesId = seriesId,
            Name = "Random"
        };

        _grandsPrixRepo.Setup(r => r.GetGrandPrixById(gp.Id)).ReturnsAsync(gp);
        _seriesRepo.Setup(r => r.GetSeriesById(seriesId)).ReturnsAsync((Series?)null);

        var result = await _svc.GetGrandPrixContext(gp.Id);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Széria nem található");
    }

    [Fact]
    public async Task GetGrandPrixContext_ShouldFail_WhenDriversChampNotFound()
    {
        var seriesId = Guid.NewGuid();
        var gp = new GrandPrix { Id = Guid.NewGuid(), SeriesId = seriesId, Name = "Random" };
        var series = CreateSeries(lastYear: 2024);

        _grandsPrixRepo.Setup(r => r.GetGrandPrixById(gp.Id)).ReturnsAsync(gp);
        _seriesRepo.Setup(r => r.GetSeriesById(seriesId)).ReturnsAsync(series);
        _driverChampRepo.Setup(r => r.GetBySeriesId(seriesId))
            .ReturnsAsync([]);

        var result = await _svc.GetGrandPrixContext(gp.Id);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Egyéni bajnokság nem található");
    }

    [Fact]
    public async Task GetGrandPrixContext_ShouldReturnAvailableSessions_ForF1()
    {
        var seriesId = Guid.NewGuid();
        var champId = Guid.NewGuid();
        var consChampId = Guid.NewGuid();
        var gp = new GrandPrix
        {
            Id = Guid.NewGuid(),
            SeriesId = seriesId,
            Name = "null"
        };
        var series = CreateSeries(lastYear: 2024, pointSystem: "F1");

        _grandsPrixRepo.Setup(r => r.GetGrandPrixById(gp.Id)).ReturnsAsync(gp);
        _seriesRepo.Setup(r => r.GetSeriesById(seriesId)).ReturnsAsync(series);
        _driverChampRepo.Setup(r => r.GetBySeriesId(seriesId))
            .ReturnsAsync([CreateDriversChamp(champId, seriesId)]);
        _constructorChampRepo.Setup(r => r.GetBySeriesId(seriesId))
            .ReturnsAsync([CreateConstructorsChamp(consChampId, seriesId)]);
        _resultsRepo.Setup(r => r.GetByGrandPrixId(gp.Id))
            .ReturnsAsync([]);

        var result = await _svc.GetGrandPrixContext(gp.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value!.AvailableSessions.Should().Contain("Futam");
        result.Value.AvailableSessions.Should().Contain("Időmérő");
        result.Value.PointSystem.Should().Be("F1");
    }

    [Fact]
    public async Task GetGrandPrixContext_ShouldExcludeCompletedSessions()
    {
        var seriesId = Guid.NewGuid();
        var champId = Guid.NewGuid();
        var consChampId = Guid.NewGuid();
        var gp = new GrandPrix
        {
            Id = Guid.NewGuid(),
            SeriesId = seriesId,
            Name = "Random"
        };
        var series = CreateSeries(lastYear: 2024, pointSystem: "F1");

        _grandsPrixRepo.Setup(r => r.GetGrandPrixById(gp.Id)).ReturnsAsync(gp);
        _seriesRepo.Setup(r => r.GetSeriesById(seriesId)).ReturnsAsync(series);
        _driverChampRepo.Setup(r => r.GetBySeriesId(seriesId))
            .ReturnsAsync([CreateDriversChamp(champId, seriesId)]);
        _constructorChampRepo.Setup(r => r.GetBySeriesId(seriesId))
            .ReturnsAsync([CreateConstructorsChamp(consChampId, seriesId)]);
        _resultsRepo.Setup(r => r.GetByGrandPrixId(gp.Id))
            .ReturnsAsync([
                new Result
                {
                    Id = Guid.NewGuid(),
                    Session = "Futam",
                    DriverNameSnapshot = "null",
                    ConstructorNameSnapshot = "null",
                    ConstructorNicknameSnapshot = "null",
                    Status = "null"
                }
            ]);

        var result = await _svc.GetGrandPrixContext(gp.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value!.AvailableSessions.Should().NotContain("Futam");
    }

    // ─────────────────────────────────────────────
    // GetGrandPrixResults
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetGrandPrixResults_ShouldFail_WhenGrandPrixNotFound()
    {
        var gpId = Guid.NewGuid();
        _grandsPrixRepo.Setup(r => r.GetGrandPrixById(gpId)).ReturnsAsync((GrandPrix?)null);

        var result = await _svc.GetGrandPrixResults(gpId, "Futam");

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("A nagydíj nem létezik");
    }

    [Fact]
    public async Task GetGrandPrixResults_ShouldReturnEmpty_WhenNoResultsForSession()
    {
        var seriesId = Guid.NewGuid();
        var gpId = Guid.NewGuid();
        var gp = new GrandPrix
        {
            Id = gpId,
            SeriesId = seriesId,
            Name = "Random"
        };

        _grandsPrixRepo.Setup(r => r.GetGrandPrixById(gpId)).ReturnsAsync(gp);
        _seriesRepo.Setup(r => r.GetSeriesById(seriesId)).ReturnsAsync(CreateSeries());
        _resultsRepo.Setup(r => r.GetBySession(gpId, "Futam")).ReturnsAsync(new List<Result>());

        var result = await _svc.GetGrandPrixResults(gpId, "Futam");

        result.IsSuccess.Should().BeTrue();
        result.Value!.Results.Should().BeEmpty();
    }

    [Fact]
    public async Task GetGrandPrixResults_ShouldReturnResults_OrderedByPosition()
    {
        var seriesId = Guid.NewGuid();
        var gpId = Guid.NewGuid();
        var gp = new GrandPrix
        {
            Id = gpId,
            SeriesId = seriesId,
            Name = "Random"
        };

        _grandsPrixRepo.Setup(r => r.GetGrandPrixById(gpId)).ReturnsAsync(gp);
        _seriesRepo.Setup(r => r.GetSeriesById(seriesId)).ReturnsAsync(CreateSeries());
        _resultsRepo.Setup(r => r.GetBySession(gpId, "Futam")).ReturnsAsync([
            CreateResult(finishPosition: 2, driverName: "Hamilton", driverPoints: 18, raceTime: 5000000),
            CreateResult(finishPosition: 1, driverName: "Verstappen", driverPoints: 25, raceTime: 4900000)
        ]);

        var result = await _svc.GetGrandPrixResults(gpId, "Futam");

        result.IsSuccess.Should().BeTrue();
        result.Value!.Results.Should().HaveCount(2);
        result.Value.Results[0].Position.Should().Be(1);
        result.Value.Results[1].Position.Should().Be(2);
    }

    // ─────────────────────────────────────────────
    // GetSeasonOverview
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetSeasonOverview_ShouldReturnOnlyRaceWinners()
    {
        var champId = Guid.NewGuid();
        var gp1 = CreateGrandPrix(round: 1, name: "Bahrain GP");
        var gp2 = CreateGrandPrix(round: 2, name: "Saudi GP");

        _resultsRepo.Setup(r => r.GetByDriversChampionshipId(champId)).ReturnsAsync([
            CreateResult(finishPosition: 1, driverName: "Verstappen", session: "Futam", gp: gp1),
            CreateResult(finishPosition: 2, driverName: "Hamilton", session: "Futam", gp: gp1),
            CreateResult(finishPosition: 1, driverName: "Leclerc", session: "Futam", gp: gp2)
        ]);

        var result = await _svc.GetSeasonOverview(champId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().AllSatisfy(r => r.WinnerName.Should().NotBe("Hamilton"));
    }

    [Fact]
    public async Task GetSeasonOverview_ShouldReturnEmpty_WhenNoRaceResults()
    {
        var champId = Guid.NewGuid();
        _resultsRepo.Setup(r => r.GetByDriversChampionshipId(champId)).ReturnsAsync(new List<Result>());

        var result = await _svc.GetSeasonOverview(champId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    // ─────────────────────────────────────────────
    // GetSessionsByGrandPrix
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetSessionsByGrandPrix_ShouldReturnAvailableSessions()
    {
        var gpId = Guid.NewGuid();
        _resultsRepo.Setup(r => r.GetAvailableSessionsByGrandPrixId(gpId))
            .ReturnsAsync(["Futam", "Időmérő"]);

        var result = await _svc.GetSessionsByGrandPrix(gpId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().Contain("Futam");
        result.Value.Should().Contain("Időmérő");
    }

    // ─────────────────────────────────────────────
    // GetDriverResultsBySeason
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetDriverResultsBySeason_ShouldReturnOnlyFutamResults_ForGivenDriver()
    {
        var champId = Guid.NewGuid();
        var driverId = Guid.NewGuid();
        var gp1 = CreateGrandPrix(round: 1, name: "Bahrain GP");
        var gp2 = CreateGrandPrix(round: 2, name: "Saudi GP");

        _resultsRepo.Setup(r => r.GetByDriversChampionshipId(champId)).ReturnsAsync([
            CreateResult(driverId: driverId, finishPosition: 1, driverPoints: 25, session: "Futam", gp: gp1),
            CreateResult(driverId: driverId, finishPosition: 2, driverPoints: 0, session: "Időmérő", gp: gp1),
            CreateResult(driverId: driverId, finishPosition: 3, driverPoints: 15, session: "Futam", gp: gp2),
            CreateResult(driverId: Guid.NewGuid(), finishPosition: 1, driverPoints: 25, session: "Futam", gp: gp1)
        ]);

        var result = await _svc.GetDriverResultsBySeason(driverId, champId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    // ─────────────────────────────────────────────
    // GetConstructorResultsBySeason
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetConstructorResultsBySeason_ShouldReturnOnlyFutamResults_ForGivenConstructor()
    {
        var champId = Guid.NewGuid();
        var constructorId = Guid.NewGuid();
        var gp1 = CreateGrandPrix(round: 1, name: "Bahrain GP");
        var gp2 = CreateGrandPrix(round: 2, name: "Saudi GP");

        _resultsRepo.Setup(r => r.GetByConstructorsChampionshipId(champId)).ReturnsAsync([
            CreateResult(constructorId: constructorId, constructorPoints: 25, session: "Futam", finishPosition: 1,
                gp: gp1),

            CreateResult(constructorId: constructorId, constructorPoints: 0, session: "Időmérő", finishPosition: 2,
                gp: gp1),

            CreateResult(constructorId: constructorId, constructorPoints: 18, session: "Futam", finishPosition: 2,
                gp: gp2),

            CreateResult(constructorId: Guid.NewGuid(), constructorPoints: 10, session: "Futam", finishPosition: 3,
                gp: gp1)

        ]);

        var result = await _svc.GetConstructorResultsBySeason(constructorId, champId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    // ─────────────────────────────────────────────
    // UpdateSingleResult
    // ─────────────────────────────────────────────

    [Fact]
    public async Task UpdateSingleResult_ShouldFail_WhenResultNotFound()
    {
        var resultId = Guid.NewGuid();
        _resultsRepo.Setup(r => r.GetResultById(resultId)).ReturnsAsync((Result?)null);

        var dto = new SingleResultUpdateDto(resultId, 1, "1:30.000", 53, "Finished", false, false);

        var result = await _svc.UpdateSingleResult(dto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Eredmény nem található");
    }

    [Fact]
    public async Task UpdateSingleResult_ShouldFail_WhenGrandPrixNotFound()
    {
        var gpId = Guid.NewGuid();
        var entity = new Result
        {
            Id = Guid.NewGuid(),
            GrandPrixId = gpId,
            Session = "Futam",
            DriverNameSnapshot = "null",
            ConstructorNameSnapshot = "null",
            ConstructorNicknameSnapshot = "null",
            Status = "null"
        };

        _resultsRepo.Setup(r => r.GetResultById(entity.Id)).ReturnsAsync(entity);
        _grandsPrixRepo.Setup(r => r.GetGrandPrixById(gpId)).ReturnsAsync((GrandPrix?)null);

        var dto = new SingleResultUpdateDto(entity.Id, 1, "1:30.000", 53, "Finished", false, false);

        var result = await _svc.UpdateSingleResult(dto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Nagydíj nem található");
    }

    // ─────────────────────────────────────────────
    // RecalculateSession
    // ─────────────────────────────────────────────

    [Fact]
    public async Task RecalculateSession_ShouldFail_WhenGrandPrixNotFound()
    {
        var gpId = Guid.NewGuid();
        _grandsPrixRepo.Setup(r => r.GetGrandPrixById(gpId)).ReturnsAsync((GrandPrix?)null);

        var result = await _svc.RecalculateSession(gpId, "Futam");

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Nagydíj nem található");
    }

    [Fact]
    public async Task RecalculateSession_ShouldFail_WhenNoResultsInSession()
    {
        var seriesId = Guid.NewGuid();
        var gpId = Guid.NewGuid();
        var gp = new GrandPrix
        {
            Id = gpId,
            SeriesId = seriesId,
            Name = "Random"
        };

        _grandsPrixRepo.Setup(r => r.GetGrandPrixById(gpId)).ReturnsAsync(gp);
        _seriesRepo.Setup(r => r.GetSeriesById(seriesId)).ReturnsAsync(CreateSeries());
        _resultsRepo.Setup(r => r.GetBySession(gpId, "Futam")).ReturnsAsync(new List<Result>());

        var result = await _svc.RecalculateSession(gpId, "Futam");

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Nincs eredmény ebben a sessionben");
    }

    [Fact]
    public async Task RecalculateSession_ShouldSucceed_AndCallUpdateForEachResult()
    {
        var seriesId = Guid.NewGuid();
        var gpId = Guid.NewGuid();
        var gp = new GrandPrix
        {
            Id = gpId,
            SeriesId = seriesId,
            Name = "Random"
        };
        
        
        var series = CreateSeries(pointSystem: "F1");

        var r1 = new Result
        {
            Id = Guid.NewGuid(),
            GrandPrixId = gpId,
            FinishPosition = 1,
            Session = "Futam",
            DriverId = Guid.NewGuid(),
            DriverNameSnapshot = "null",
            ConstructorNameSnapshot = "null",
            ConstructorNicknameSnapshot = "null",
            Status = "null"
        };
        var r2 = new Result
        {
            Id = Guid.NewGuid(),
            GrandPrixId = gpId,
            FinishPosition = 2,
            Session = "Futam",
            DriverId = Guid.NewGuid(),
            DriverNameSnapshot = "null",
            ConstructorNameSnapshot = "null",
            ConstructorNicknameSnapshot = "null",
            Status = "null"
        };

        _grandsPrixRepo.Setup(r => r.GetGrandPrixById(gpId)).ReturnsAsync(gp);
        _seriesRepo.Setup(r => r.GetSeriesById(seriesId)).ReturnsAsync(series);
        _resultsRepo.Setup(r => r.GetBySession(gpId, It.IsAny<string>())).ReturnsAsync([]);
        _resultsRepo.Setup(r => r.GetBySession(gpId, "Futam")).ReturnsAsync([r1, r2]);
        _resultsRepo.Setup(r => r.GetAvailableSessionsByGrandPrixId(gpId)).ReturnsAsync(["Futam"]);
        _resultsRepo.Setup(r => r.Update(It.IsAny<Result>())).Returns(Task.CompletedTask);

        var result = await _svc.RecalculateSession(gpId, "Futam");

        result.IsSuccess.Should().BeTrue();
        _resultsRepo.Verify(r => r.Update(It.IsAny<Result>()), Times.AtLeast(2));
    }

    // ─────────────────────────────────────────────
    // InsertResults
    // ─────────────────────────────────────────────

    [Fact]
    public async Task InsertResults_ShouldFail_WhenGrandPrixNotFound()
    {
        var gpId = Guid.NewGuid();
        _grandsPrixRepo.Setup(r => r.GetGrandPrixById(gpId)).ReturnsAsync((GrandPrix?)null);

        var dto = new BatchResultCreateDto(gpId, Guid.NewGuid(), Guid.NewGuid(), "Futam",
            new List<SingleResultCreateDto>());

        var result = await _svc.InsertResults(dto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Nagydíj nem található");
    }

    [Fact]
    public async Task InsertResults_ShouldFail_WhenSeriesNotFound()
    {
        var seriesId = Guid.NewGuid();
        var gpId = Guid.NewGuid();
        var gp = new GrandPrix
        {
            Id = gpId,
            SeriesId = seriesId,
            Name = "Random"
        };

        _grandsPrixRepo.Setup(r => r.GetGrandPrixById(gpId)).ReturnsAsync(gp);
        _seriesRepo.Setup(r => r.GetSeriesById(seriesId)).ReturnsAsync((Series?)null);

        var dto = new BatchResultCreateDto(gpId, Guid.NewGuid(), Guid.NewGuid(), "Futam",
            new List<SingleResultCreateDto>());

        var result = await _svc.InsertResults(dto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Széria nem található");
    }

    // ─────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────

    private static Series CreateSeries(
        string name = "Formula 1",
        string shortName = "F1",
        string pointSystem = "F1",
        int lastYear = 2024) => new()
    {
        Id = Guid.NewGuid(),
        Name = name,
        ShortName = shortName,
        PointSystem = pointSystem,
        GoverningBody = "FIA",
        Description = "Top motorsport",
        FirstYear = 1950,
        LastYear = lastYear
    };

    private static GrandPrix CreateGrandPrix(int round = 1, string name = "Test GP") => new()
    {
        Id = Guid.NewGuid(),
        Name = name,
        ShortName = name.Length >= 3 ? name[..3].ToUpper() : "TST",
        RoundNumber = round,
        SeasonYear = 2024,
        SeriesId = Guid.NewGuid(),
        EndTime = DateTime.UtcNow
    };

    private static DriversChampionship CreateDriversChamp(Guid id, Guid seriesId, string season = "2024") => new()
    {
        Id = id,
        SeriesId = seriesId,
        Season = season,
        Name = "WDC",
        Status = "Active"
    };

    private static ConstructorsChampionship CreateConstructorsChamp(Guid id, Guid seriesId, string season = "2024") =>
        new()
        {
            Id = id,
            SeriesId = seriesId,
            Season = season,
            Name = "WCC",
            Status = "Active"
        };

    private static ConstructorsChampionship CreateConstructorsChamp(Guid id) => new()
    {
        Id = id,
        Season = "2024",
        Name = "WCC",
        Status = "Ongoing"
    };

    private static Result CreateResult(
        Guid? driverId          = null,
        Guid? constructorId     = null,
        Guid? driverChampId     = null,   // ← ez volt a kulcs
        Guid? consChampId       = null,   // ← ez is
        double driverPoints     = 0,
        double constructorPoints = 0,
        int finishPosition      = 1,
        string driverName       = "Driver",
        string consName         = "Team",
        string consNick         = "TM",
        string session          = "Futam",
        long raceTime           = 5000000,
        GrandPrix? gp           = null) => new()
    {
        Id                          = Guid.NewGuid(),
        GrandPrixId                 = gp?.Id ?? Guid.NewGuid(),
        GrandPrix                   = gp,
        DriverId                    = driverId      ?? Guid.NewGuid(),
        ConstructorId               = constructorId ?? Guid.NewGuid(),
        DriversChampId              = driverChampId ?? Guid.NewGuid(),
        ConsChampId                 = consChampId   ?? Guid.NewGuid(),
        DriverNameSnapshot          = driverName,
        ConstructorNameSnapshot     = consName,
        ConstructorNicknameSnapshot = consNick,
        Session                     = session,
        FinishPosition              = finishPosition,
        DriverPoints                = driverPoints,
        ConstructorPoints           = constructorPoints,
        RaceTime                    = raceTime,
        LapsCompleted               = 53,
        Status                      = "Finished",
        CarNumber                   = 1
    };
}