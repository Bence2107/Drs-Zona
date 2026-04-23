using Entities.Models.Standings;
using FluentAssertions;
using Moq;
using Repositories.Interfaces.Standings;
using Services.Implementations;
using Xunit;

namespace Tests.Services.Units;

public class SeriesServiceUnitTests
{
    private readonly Mock<ISeriesRepository>                     _seriesRepo       = new();
    private readonly Mock<IDriversChampionshipsRepository>       _driverChampRepo  = new();
    private readonly Mock<IConstructorsChampionshipsRepository>  _constChampRepo   = new();

    private readonly SeriesService _svc;

    public SeriesServiceUnitTests()
    {
        _svc = new SeriesService(
            _seriesRepo.Object,
            _driverChampRepo.Object,
            _constChampRepo.Object
        );
    }

    // ─────────────────────────────────────────────
    // GetSeriesByName
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetSeriesByName_ShouldFail_WhenSeriesNotFound()
    {
        _seriesRepo.Setup(r => r.GetByName("F1")).ReturnsAsync((Series?)null);

        var result = await _svc.GetSeriesByName("F1");

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Series not exist");
    }

    [Fact]
    public async Task GetSeriesByName_ShouldReturnSeriesDetail_WithAvailableSeasons()
    {
        var series = CreateSeries();

        _seriesRepo.Setup(r => r.GetByName(series.ShortName)).ReturnsAsync(series);
        _driverChampRepo.Setup(r => r.GetBySeriesId(series.Id)).ReturnsAsync([
            CreateDriversChamp(series.Id, "2023"),
            CreateDriversChamp(series.Id, "2024")
        ]);
        _constChampRepo.Setup(r => r.GetBySeriesId(series.Id)).ReturnsAsync([
            CreateConstructorsChamp(series.Id, "2023"),
            CreateConstructorsChamp(series.Id, "2024")
        ]);

        var result = await _svc.GetSeriesByName(series.ShortName);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(series.Id);
        result.Value.Name.Should().Be(series.Name);
        result.Value.ShortName.Should().Be(series.ShortName);
        result.Value.AvailableSeasons.Should().HaveCount(2);
        result.Value.AvailableSeasons![0].Should().Be("2024");
        result.Value.AvailableSeasons[1].Should().Be("2023");
    }

    [Fact]
    public async Task GetSeriesByName_ShouldMergeAndDeduplicateSeasons()
    {
        var series = CreateSeries();

        // Driver champ: 2022, 2023 — Constructor champ: 2023, 2024
        // Union → 2022, 2023, 2024 (rendezett csökkenő)
        _seriesRepo.Setup(r => r.GetByName(series.ShortName)).ReturnsAsync(series);
        _driverChampRepo.Setup(r => r.GetBySeriesId(series.Id)).ReturnsAsync([
            CreateDriversChamp(series.Id, "2022"),
            CreateDriversChamp(series.Id, "2023")
        ]);
        _constChampRepo.Setup(r => r.GetBySeriesId(series.Id)).ReturnsAsync([
            CreateConstructorsChamp(series.Id, "2023"),
            CreateConstructorsChamp(series.Id, "2024")
        ]);

        var result = await _svc.GetSeriesByName(series.ShortName);

        result.IsSuccess.Should().BeTrue();
        result.Value!.AvailableSeasons.Should().HaveCount(3);
        result.Value.AvailableSeasons.Should().ContainInOrder("2024", "2023", "2022");
    }

    [Fact]
    public async Task GetSeriesByName_ShouldReturnEmptySeasons_WhenNoChampionshipsExist()
    {
        var series = CreateSeries();

        _seriesRepo.Setup(r => r.GetByName(series.ShortName)).ReturnsAsync(series);
        _driverChampRepo.Setup(r => r.GetBySeriesId(series.Id)).ReturnsAsync(new List<DriversChampionship>());
        _constChampRepo.Setup(r => r.GetBySeriesId(series.Id)).ReturnsAsync(new List<ConstructorsChampionship>());

        var result = await _svc.GetSeriesByName(series.ShortName);

        result.IsSuccess.Should().BeTrue();
        result.Value!.AvailableSeasons.Should().BeEmpty();
    }

    [Fact]
    public async Task GetSeriesByName_ShouldMapAllSeriesFields()
    {
        var series = CreateSeries();

        _seriesRepo.Setup(r => r.GetByName(series.ShortName)).ReturnsAsync(series);
        _driverChampRepo.Setup(r => r.GetBySeriesId(series.Id)).ReturnsAsync(new List<DriversChampionship>());
        _constChampRepo.Setup(r => r.GetBySeriesId(series.Id)).ReturnsAsync(new List<ConstructorsChampionship>());

        var result = await _svc.GetSeriesByName(series.ShortName);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Description.Should().Be(series.Description);
        result.Value.GoverningBody.Should().Be(series.GoverningBody);
        result.Value.FirstYear.Should().Be(series.FirstYear);
        result.Value.LastYear.Should().Be(series.LastYear);
    }

    // ─────────────────────────────────────────────
    // ListSeries
    // ─────────────────────────────────────────────

    [Fact]
    public async Task ListSeries_ShouldReturnList_OrderedByShortName()
    {
        _seriesRepo.Setup(r => r.GetAllSeries()).ReturnsAsync([
            CreateSeries("MotoGP", "MOTOGP"),
            CreateSeries(),
            CreateSeries("Formula 2", "F2")
        ]);

        var result = await _svc.ListSeries();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);
        result.Value![0].ShortName.Should().Be("F1");
        result.Value[1].ShortName.Should().Be("F2");
        result.Value[2].ShortName.Should().Be("MOTOGP");
    }

    [Fact]
    public async Task ListSeries_ShouldReturnEmpty_WhenNoSeriesExist()
    {
        _seriesRepo.Setup(r => r.GetAllSeries()).ReturnsAsync(new List<Series>());

        var result = await _svc.ListSeries();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    // ─────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────

    private static Series CreateSeries(string name = "Formula 1", string shortName = "F1") => new()
    {
        Id = Guid.NewGuid(),
        Name = name,
        ShortName = shortName,
        Description = "The pinnacle of motorsport",
        GoverningBody = "FIA",
        FirstYear = 1950,
        LastYear = 2024,
        PointSystem = "FIA"
    };

    private static DriversChampionship CreateDriversChamp(Guid seriesId, string season) => new()
    {
        Id       = Guid.NewGuid(),
        SeriesId = seriesId,
        Season   = season,
        Name     = $"WDC {season}",
        Status   = "Finished"
    };

    private static ConstructorsChampionship CreateConstructorsChamp(Guid seriesId, string season) => new()
    {
        Id       = Guid.NewGuid(),
        SeriesId = seriesId,
        Season   = season,
        Name     = $"WCC {season}",
        Status   = "Finished"
    };
}