using DTOs.RaceTracks;
using Entities.Models.RaceTracks;
using Entities.Models.Standings;
using FluentAssertions;
using Moq;
using Repositories.Interfaces.RaceTracks;
using Repositories.Interfaces.Standings;
using Services.Implementations;
using Services.Interfaces.images;
using Xunit;

namespace Tests.Services.Units;

public class GrandPrixServiceUnitTests
{

    private readonly Mock<ICircuitsRepository>    _circuitsRepo         = new();
    private readonly Mock<IGrandsPrixRepository>  _grandsPrixRepo       = new();
    private readonly Mock<ISeriesRepository>      _seriesRepo           = new();
    private readonly Mock<ICircuitImagesService>  _circuitImagesService = new();

    private readonly GrandPrixService _svc;

    public GrandPrixServiceUnitTests()
    {

        _circuitImagesService
            .Setup(s => s.GetImageUrl(It.IsAny<Guid>(), It.IsAny<string>()))
            .Returns((Guid id, string name) => $"/uploads/images/circuits/{id}/{name}.png");

        _svc = new GrandPrixService(
            _circuitsRepo.Object,
            _grandsPrixRepo.Object,
            _seriesRepo.Object,
            _circuitImagesService.Object
        );
    }

    // ─────────────────────────────────────────────
    // GetGrandPrixById
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetGrandPrixById_ShouldFail_WhenNotFound()
    {
        var id = Guid.NewGuid();
        _grandsPrixRepo.Setup(r => r.GetWithAll(id)).ReturnsAsync((GrandPrix?)null);

        var result = await _svc.GetGrandPrixById(id);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Grand Prix not found");
    }

    [Fact]
    public async Task GetGrandPrixById_ShouldFail_WhenCircuitIsNull()
    {
        var gp = CreateGrandPrix();
        gp.Circuit = null;

        _grandsPrixRepo.Setup(r => r.GetWithAll(gp.Id)).ReturnsAsync(gp);

        var result = await _svc.GetGrandPrixById(gp.Id);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Circuit is not found");
    }

    [Fact]
    public async Task GetGrandPrixById_ShouldFail_WhenSeriesIsNull()
    {
        var gp = CreateGrandPrix();
        gp.Series = null;

        _grandsPrixRepo.Setup(r => r.GetWithAll(gp.Id)).ReturnsAsync(gp);

        var result = await _svc.GetGrandPrixById(gp.Id);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Series is not found");
    }

    [Fact]
    public async Task GetGrandPrixById_ShouldReturnDetailDto_WithCorrectFields()
    {
        var gp = CreateGrandPrix();

        _grandsPrixRepo.Setup(r => r.GetWithAll(gp.Id)).ReturnsAsync(gp);

        var result = await _svc.GetGrandPrixById(gp.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(gp.Id);
        result.Value.Name.Should().Be(gp.Name);
        result.Value.SeriesName.Should().Be(gp.Series!.Name);
        result.Value.RoundNumber.Should().Be(gp.RoundNumber);
        result.Value.SeasonYear.Should().Be(gp.SeasonYear);
        result.Value.RaceDistance.Should().Be(gp.RaceDistance);
    }

    [Fact]
    public async Task GetGrandPrixById_ShouldReturnCircuitDetailDto_WithImageUrls()
    {
        var gp = CreateGrandPrix();

        _grandsPrixRepo.Setup(r => r.GetWithAll(gp.Id)).ReturnsAsync(gp);

        var result = await _svc.GetGrandPrixById(gp.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value!.CircuitDetail.Id.Should().Be(gp.Circuit!.Id);
        result.Value.CircuitDetail.LightImageUrl.Should().Contain("light");
        result.Value.CircuitDetail.DarkImageUrl.Should().Contain("dark");
    }

    // ─────────────────────────────────────────────
    // GetAllCircuits
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetAllCircuits_ShouldReturnAllCircuits()
    {
        var circuits = new List<Circuit>
        {
            CreateCircuit(),
            CreateCircuit("Silverstone", "UK"),
        };

        _circuitsRepo.Setup(r => r.GetAllCircuits()).ReturnsAsync(circuits);

        var result = await _svc.GetAllCircuits();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value![0].Name.Should().Be("Monza");
        result.Value[1].Name.Should().Be("Silverstone");
    }

    [Fact]
    public async Task GetAllCircuits_ShouldReturnEmpty_WhenNoneExist()
    {
        _circuitsRepo.Setup(r => r.GetAllCircuits()).ReturnsAsync(new List<Circuit>());

        var result = await _svc.GetAllCircuits();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    // ─────────────────────────────────────────────
    // GetSeasonGrandPrixList
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetSeasonGrandPrixList_ShouldReturnGrandsPrix_ForGivenSeriesAndYear()
    {
        var seriesId = Guid.NewGuid();
        var gps = new List<GrandPrix>
        {
            CreateGrandPrix(seriesId: seriesId, seasonYear: 2024, round: 1),
            CreateGrandPrix(seriesId: seriesId, seasonYear: 2024, round: 2),
        };

        _grandsPrixRepo.Setup(r => r.GetBySeriesAndYear(seriesId, 2024)).ReturnsAsync(gps);

        var result = await _svc.GetSeasonGrandPrixList(seriesId, 2024);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value![0].RoundNumber.Should().Be(1);
        result.Value[1].RoundNumber.Should().Be(2);
    }

    [Fact]
    public async Task GetSeasonGrandPrixList_ShouldReturnEmpty_WhenNoGrandsPrixExist()
    {
        _grandsPrixRepo.Setup(r => r.GetBySeriesAndYear(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync(new List<GrandPrix>());

        var result = await _svc.GetSeasonGrandPrixList(Guid.NewGuid(), 2024);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    // ─────────────────────────────────────────────
    // Create
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Create_ShouldFail_WhenCircuitNotFound()
    {
        var circuitId = Guid.NewGuid();
        _circuitsRepo.Setup(r => r.GetCircuitById(circuitId)).ReturnsAsync((Circuit?)null);

        var dto = CreateGrandPrixCreateDto(circuitId: circuitId);

        var result = await _svc.Create(dto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("The specified circuit does not exist.");
    }

    [Fact]
    public async Task Create_ShouldFail_WhenSeriesNotFound()
    {
        var circuitId = Guid.NewGuid();
        var seriesId  = Guid.NewGuid();

        _circuitsRepo.Setup(r => r.GetCircuitById(circuitId)).ReturnsAsync(CreateCircuit());
        _seriesRepo.Setup(r => r.GetSeriesById(seriesId)).ReturnsAsync((Series?)null);

        var dto = CreateGrandPrixCreateDto(circuitId: circuitId, seriesId: seriesId);

        var result = await _svc.Create(dto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("The specified series does not exist.");
    }

    [Fact]
    public async Task Create_ShouldSucceed_AndCallRepoCreate()
    {
        var circuitId = Guid.NewGuid();
        var seriesId  = Guid.NewGuid();

        _circuitsRepo.Setup(r => r.GetCircuitById(circuitId)).ReturnsAsync(CreateCircuit());
        _seriesRepo.Setup(r => r.GetSeriesById(seriesId)).ReturnsAsync(CreateSeries());
        _grandsPrixRepo.Setup(r => r.Create(It.IsAny<GrandPrix>())).Returns(Task.CompletedTask);

        var dto = CreateGrandPrixCreateDto(circuitId: circuitId, seriesId: seriesId);

        var result = await _svc.Create(dto);

        result.IsSuccess.Should().BeTrue();
        _grandsPrixRepo.Verify(r => r.Create(It.Is<GrandPrix>(gp =>
            gp.CircuitId == circuitId &&
            gp.SeriesId  == seriesId  &&
            gp.Name      == dto.Name
        )), Times.Once);
    }

    // ─────────────────────────────────────────────
    // Update
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Update_ShouldFail_WhenGrandPrixNotFound()
    {
        var id = Guid.NewGuid();
        _grandsPrixRepo.Setup(r => r.GetGrandPrixById(id)).ReturnsAsync((GrandPrix?)null);

        var dto = new GrandPrixUpdateDto(id, DateTime.Now, DateTime.Now.AddHours(2), 58);

        var result = await _svc.Update(dto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Grand Prix not found");
    }

    [Fact]
    public async Task Update_ShouldSucceed_AndCallRepoUpdate()
    {
        var gp = CreateGrandPrix();
        _grandsPrixRepo.Setup(r => r.GetGrandPrixById(gp.Id)).ReturnsAsync(gp);
        _grandsPrixRepo.Setup(r => r.Update(It.IsAny<GrandPrix>())).Returns(Task.CompletedTask);

        var newStart = DateTime.Now.AddHours(1);
        var newEnd   = DateTime.Now.AddHours(3);
        var dto      = new GrandPrixUpdateDto(gp.Id, newStart, newEnd, 60);

        var result = await _svc.Update(dto);

        result.IsSuccess.Should().BeTrue();
        _grandsPrixRepo.Verify(r => r.Update(It.Is<GrandPrix>(g =>
            g.StartTime    == newStart &&
            g.EndTime      == newEnd   &&
            g.LapsCompleted == 60
        )), Times.Once);
    }

    [Fact]
    public async Task Update_ShouldOnlyUpdateTimesAndLaps_NotOtherFields()
    {
        var gp = CreateGrandPrix();
        GrandPrix? updated = null;

        _grandsPrixRepo.Setup(r => r.GetGrandPrixById(gp.Id)).ReturnsAsync(gp);
        _grandsPrixRepo
            .Setup(r => r.Update(It.IsAny<GrandPrix>()))
            .Callback<GrandPrix>(g => updated = g)
            .Returns(Task.CompletedTask);

        var dto = new GrandPrixUpdateDto(gp.Id, DateTime.Now.AddHours(1), DateTime.Now.AddHours(3), 60);

        await _svc.Update(dto);

        updated!.Name.Should().Be(gp.Name);
        updated.RoundNumber.Should().Be(gp.RoundNumber);
        updated.SeasonYear.Should().Be(gp.SeasonYear);
    }

    // ─────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────

    private static Circuit CreateCircuit(string name = "Monza", string location = "Italy") => new()
    {
        Id         = Guid.NewGuid(),
        Name       = name,
        Location   = location,
        Length     = 5.793,
        Type       = "Permanent",
        FastestLap = "1:21.046"
    };

    private static Series CreateSeries() => new()
    {
        Id = Guid.NewGuid(),
        Name = "Formula 1",
        ShortName = "F1",
        Description = "Top tier motorsport",
        GoverningBody = "FIA",
        FirstYear = 1950,
        LastYear = 2024,
        PointSystem = "FIA"
    };

    private static GrandPrix CreateGrandPrix(
        Guid?  seriesId   = null,
        int    seasonYear = 2024,
        int    round      = 1) => new()
    {
        Id            = Guid.NewGuid(),
        SeriesId      = seriesId ?? Guid.NewGuid(),
        Name          = "Italian Grand Prix",
        ShortName     = "ITA",
        RoundNumber   = round,
        SeasonYear    = seasonYear,
        StartTime     = DateTime.UtcNow,
        EndTime       = DateTime.UtcNow.AddHours(2),
        RaceDistance  = 306,
        LapsCompleted = 53,
        Circuit       = CreateCircuit(),
        Series        = CreateSeries()
    };

    private static GrandPrixCreateDto CreateGrandPrixCreateDto(
        Guid?  circuitId = null,
        Guid?  seriesId  = null) => new(
        circuitId ?? Guid.NewGuid(),
        seriesId  ?? Guid.NewGuid(),
        "Italian Grand Prix",
        "ITA",
        1,
        2024,
        DateTime.UtcNow,
        DateTime.UtcNow.AddHours(2),
        306,
        53
    );
}
