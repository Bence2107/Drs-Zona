using Context;
using DTOs.RaceTracks;
using Entities.Models.RaceTracks;
using Entities.Models.Standings;
using FluentAssertions;
using Moq;
using Repositories.Implementations.RaceTracks;
using Repositories.Implementations.Standings;
using Services.Implementations;
using Services.Interfaces;
using Services.Interfaces.images;
using Xunit;

namespace Tests.Services.Integrations;

public class GrandPrixServiceIntegrationTests
{

    // CircuitImageService fájlrendszert használ — mindig mockolt
    private static (EfContext ctx, IGrandPrixService svc) BuildSut()
    {
        var ctx = InMemoryDbFactory.CreateContext();

        var circuitImagesMock = new Mock<ICircuitImagesService>();
        circuitImagesMock
            .Setup(s => s.GetImageUrl(It.IsAny<Guid>(), It.IsAny<string>()))
            .Returns((Guid id, string name) => $"/uploads/images/circuits/{id}/{name}.png");

        var svc = new GrandPrixService(
            new CircuitsRepository(ctx),
            new GrandsPrixRepository(ctx),
            new SeriesRepository(ctx),
            circuitImagesMock.Object
        );

        return (ctx, svc);
    }

    // ─────────────────────────────────────────────
    // GetGrandPrixById
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetGrandPrixById_ShouldFail_WhenNotFound()
    {
        var (_, svc) = BuildSut();

        var result = await svc.GetGrandPrixById(Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Grand Prix not found");
    }

    [Fact]
    public async Task GetGrandPrixById_ShouldReturnDetailDto_WithCorrectFields()
    {
        var (ctx, svc) = BuildSut();

        var circuit = CreateCircuit();
        var series  = CreateSeries();
        var gp      = CreateGrandPrix(circuit.Id, series.Id);

        await ctx.Circuits.AddAsync(circuit);
        await ctx.Series.AddAsync(series);
        await ctx.GrandsPrix.AddAsync(gp);
        await ctx.SaveChangesAsync();

        var result = await svc.GetGrandPrixById(gp.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(gp.Id);
        result.Value.Name.Should().Be(gp.Name);
        result.Value.SeriesName.Should().Be(series.Name);
        result.Value.RoundNumber.Should().Be(gp.RoundNumber);
        result.Value.SeasonYear.Should().Be(gp.SeasonYear);
        result.Value.RaceDistance.Should().Be(gp.RaceDistance);
    }

    [Fact]
    public async Task GetGrandPrixById_ShouldReturnCircuitDetail_WithImageUrls()
    {
        var (ctx, svc) = BuildSut();

        var circuit = CreateCircuit();
        var series  = CreateSeries();
        var gp      = CreateGrandPrix(circuit.Id, series.Id);

        await ctx.Circuits.AddAsync(circuit);
        await ctx.Series.AddAsync(series);
        await ctx.GrandsPrix.AddAsync(gp);
        await ctx.SaveChangesAsync();

        var result = await svc.GetGrandPrixById(gp.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value!.CircuitDetail.Id.Should().Be(circuit.Id);
        result.Value.CircuitDetail.Name.Should().Be(circuit.Name);
        result.Value.CircuitDetail.LightImageUrl.Should().Contain("light");
        result.Value.CircuitDetail.DarkImageUrl.Should().Contain("dark");
    }

    // ─────────────────────────────────────────────
    // GetAllCircuits
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetAllCircuits_ShouldReturnAll()
    {
        var (ctx, svc) = BuildSut();

        await ctx.Circuits.AddRangeAsync(
            CreateCircuit(),
            CreateCircuit("Silverstone", "UK"),
            CreateCircuit("Spa",         "Belgium")
        );
        await ctx.SaveChangesAsync();

        var result = await svc.GetAllCircuits();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAllCircuits_ShouldReturnEmpty_WhenNoneExist()
    {
        var (_, svc) = BuildSut();

        var result = await svc.GetAllCircuits();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllCircuits_ShouldMapLocationAndName()
    {
        var (ctx, svc) = BuildSut();

        var circuit = CreateCircuit();
        await ctx.Circuits.AddAsync(circuit);
        await ctx.SaveChangesAsync();

        var result = await svc.GetAllCircuits();

        result.IsSuccess.Should().BeTrue();
        result.Value![0].Name.Should().Be("Monza");
        result.Value[0].Location.Should().Be("Italy");
    }

    // ─────────────────────────────────────────────
    // GetSeasonGrandPrixList
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetSeasonGrandPrixList_ShouldReturnGrandsPrix_ForCorrectSeriesAndYear()
    {
        var (ctx, svc) = BuildSut();

        var circuit  = CreateCircuit();
        var series   = CreateSeries();
        var otherSeries = CreateSeries("MotoGP", "MOTOGP");

        await ctx.Circuits.AddAsync(circuit);
        await ctx.Series.AddRangeAsync(series, otherSeries);

        await ctx.GrandsPrix.AddRangeAsync(
            CreateGrandPrix(circuit.Id, series.Id,      seasonYear: 2024, round: 1),
            CreateGrandPrix(circuit.Id, series.Id,      seasonYear: 2024, round: 2),
            CreateGrandPrix(circuit.Id, series.Id,      seasonYear: 2023, round: 1),
            CreateGrandPrix(circuit.Id, otherSeries.Id, seasonYear: 2024, round: 1)
        );
        await ctx.SaveChangesAsync();

        var result = await svc.GetSeasonGrandPrixList(series.Id, 2024);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().AllSatisfy(gp => gp.SeasonYear.Should().Be(2024));
    }

    [Fact]
    public async Task GetSeasonGrandPrixList_ShouldReturnEmpty_WhenNoMatch()
    {
        var (_, svc) = BuildSut();

        var result = await svc.GetSeasonGrandPrixList(Guid.NewGuid(), 2024);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    // ─────────────────────────────────────────────
    // Create
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Create_ShouldSucceed_AndPersistGrandPrix()
    {
        var (ctx, svc) = BuildSut();

        var circuit = CreateCircuit();
        var series  = CreateSeries();
        await ctx.Circuits.AddAsync(circuit);
        await ctx.Series.AddAsync(series);
        await ctx.SaveChangesAsync();

        var dto = new GrandPrixCreateDto(
            circuit.Id, series.Id,
            "Italian Grand Prix", "ITA",
            1, 2024,
            DateTime.UtcNow, DateTime.UtcNow.AddHours(2),
            306, 53
        );

        var result = await svc.Create(dto);

        result.IsSuccess.Should().BeTrue();
        ctx.ChangeTracker.Clear();
        ctx.GrandsPrix.Should().HaveCount(1);
        ctx.GrandsPrix.First().Name.Should().Be("Italian Grand Prix");
        ctx.GrandsPrix.First().CircuitId.Should().Be(circuit.Id);
    }

    [Fact]
    public async Task Create_ShouldFail_WhenCircuitNotFound()
    {
        var (ctx, svc) = BuildSut();

        var series = CreateSeries();
        await ctx.Series.AddAsync(series);
        await ctx.SaveChangesAsync();

        var dto = new GrandPrixCreateDto(
            Guid.NewGuid(), series.Id,
            "Italian Grand Prix", "ITA",
            1, 2024,
            DateTime.UtcNow, DateTime.UtcNow.AddHours(2),
            306, 53
        );

        var result = await svc.Create(dto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("The specified circuit does not exist.");
    }

    [Fact]
    public async Task Create_ShouldFail_WhenSeriesNotFound()
    {
        var (ctx, svc) = BuildSut();

        var circuit = CreateCircuit();
        await ctx.Circuits.AddAsync(circuit);
        await ctx.SaveChangesAsync();

        var dto = new GrandPrixCreateDto(
            circuit.Id, Guid.NewGuid(),
            "Italian Grand Prix", "ITA",
            1, 2024,
            DateTime.UtcNow, DateTime.UtcNow.AddHours(2),
            306, 53
        );

        var result = await svc.Create(dto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("The specified series does not exist.");
    }

    // ─────────────────────────────────────────────
    // Update
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Update_ShouldSucceed_AndPersistChanges()
    {
        var (ctx, svc) = BuildSut();

        var circuit = CreateCircuit();
        var series  = CreateSeries();
        var gp      = CreateGrandPrix(circuit.Id, series.Id);

        await ctx.Circuits.AddAsync(circuit);
        await ctx.Series.AddAsync(series);
        await ctx.GrandsPrix.AddAsync(gp);
        await ctx.SaveChangesAsync();

        var newStart = DateTime.UtcNow.AddHours(1);
        var newEnd   = DateTime.UtcNow.AddHours(3);
        var dto      = new GrandPrixUpdateDto(gp.Id, newStart, newEnd, 60);

        var result = await svc.Update(dto);

        result.IsSuccess.Should().BeTrue();
        ctx.ChangeTracker.Clear();
        var updated = await ctx.GrandsPrix.FindAsync(gp.Id);
        updated!.StartTime.Should().BeCloseTo(newStart, TimeSpan.FromSeconds(1));
        updated.EndTime.Should().BeCloseTo(newEnd, TimeSpan.FromSeconds(1));
        updated.LapsCompleted.Should().Be(60);
    }

    [Fact]
    public async Task Update_ShouldFail_WhenGrandPrixNotFound()
    {
        var (_, svc) = BuildSut();

        var dto = new GrandPrixUpdateDto(Guid.NewGuid(), DateTime.UtcNow, DateTime.UtcNow.AddHours(2), 58);

        var result = await svc.Update(dto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Grand Prix not found");
    }

    [Fact]
    public async Task Update_ShouldNotChangeOtherFields()
    {
        var (ctx, svc) = BuildSut();

        var circuit = CreateCircuit();
        var series  = CreateSeries();
        var gp      = CreateGrandPrix(circuit.Id, series.Id, round: 5, seasonYear: 2024);
        gp.Name = "Original Name";

        await ctx.Circuits.AddAsync(circuit);
        await ctx.Series.AddAsync(series);
        await ctx.GrandsPrix.AddAsync(gp);
        await ctx.SaveChangesAsync();

        var dto = new GrandPrixUpdateDto(gp.Id, DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(3), 58);

        await svc.Update(dto);

        ctx.ChangeTracker.Clear();
        var updated = await ctx.GrandsPrix.FindAsync(gp.Id);
        updated!.Name.Should().Be("Original Name");
        updated.RoundNumber.Should().Be(5);
        updated.SeasonYear.Should().Be(2024);
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

    private static Series CreateSeries(string name = "Formula 1", string shortName = "F1") => new()
    {
        Id = Guid.NewGuid(),
        Name = name,
        ShortName = shortName,
        Description = "Top tier motorsport",
        GoverningBody = "FIA",
        FirstYear = 1950,
        LastYear = 2024,
        PointSystem = "FIA"
    };

    private static GrandPrix CreateGrandPrix(
        Guid circuitId,
        Guid seriesId,
        int  round      = 1,
        int  seasonYear = 2024) => new()
    {
        Id            = Guid.NewGuid(),
        CircuitId     = circuitId,
        SeriesId      = seriesId,
        Name          = "Italian Grand Prix",
        ShortName     = "ITA",
        RoundNumber   = round,
        SeasonYear    = seasonYear,
        StartTime     = DateTime.UtcNow,
        EndTime       = DateTime.UtcNow.AddHours(2),
        RaceDistance  = 306,
        LapsCompleted = 53
    };
}
