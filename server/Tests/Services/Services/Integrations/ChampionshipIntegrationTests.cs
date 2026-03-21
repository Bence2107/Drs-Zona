using Context;
using DTOs.Standings;
using Entities.Models.RaceTracks;
using Entities.Models.Standings;
using FluentAssertions;
using Repositories.Implementations.RaceTracks;
using Repositories.Implementations.Standings;
using Services.Implementations;
using Services.Interfaces;
using Xunit;

namespace Tests.Services.Integrations;

public class ChampionshipIntegrationTests
{

    private static (EfContext ctx, IChampionshipService svc) BuildSut()
    {
        var ctx = InMemoryDbFactory.CreateContext();

     
        var svc = new ChampionshipService(
            new ConstructorsRepository(ctx),
            new ConstructorsChampionshipsRepository(ctx),
            new ConstructorCompetitionRepository(ctx),
            new ContractsRepository(ctx),
            new DriversRepository(ctx),
            new DriversChampionshipsRepository(ctx),
            new DriverParticipationRepository(ctx),
            new GrandsPrixRepository(ctx),
            new ResultsRepository(ctx)
        );

        return (ctx, svc);
    }

    // ─────────────────────────────────────────────
    // GetAllChampionshipsBySeries
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetAllChampionshipsBySeries_ShouldReturnRows_OrderedBySeasonDescending()
    {
        var (ctx, svc) = BuildSut();

        var series = CreateSeries();
        await ctx.Series.AddAsync(series);

        var dc2023 = CreateDriversChamp(series.Id, "2023", "WDC 2023");
        var dc2024 = CreateDriversChamp(series.Id, "2024", "WDC 2024");
        var cc2023 = CreateConstructorsChamp(series.Id, "2023", "WCC 2023");
        var cc2024 = CreateConstructorsChamp(series.Id, "2024", "WCC 2024");

        await ctx.DriversChampionships.AddRangeAsync(dc2023, dc2024);
        await ctx.ConstructorsChampionships.AddRangeAsync(cc2023, cc2024);
        await ctx.SaveChangesAsync();

        var result = await svc.GetAllChampionshipsBySeries(series.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value![0].Season.Should().Be("2024");
        result.Value[1].Season.Should().Be("2023");
    }

    [Fact]
    public async Task GetAllChampionshipsBySeries_ShouldReturnEmpty_WhenNoneExist()
    {
        var (_, svc) = BuildSut();

        var result = await svc.GetAllChampionshipsBySeries(Guid.NewGuid());

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    // ─────────────────────────────────────────────
    // GetSeasonsBySeries
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetSeasonsBySeries_ShouldReturnYearLookups_WithCorrectIds()
    {
        var (ctx, svc) = BuildSut();

        var series = CreateSeries();
        await ctx.Series.AddAsync(series);

        var dc = CreateDriversChamp(series.Id, "2024", "WDC 2024");
        var cc = CreateConstructorsChamp(series.Id, "2024", "WCC 2024");
        await ctx.DriversChampionships.AddAsync(dc);
        await ctx.ConstructorsChampionships.AddAsync(cc);
        await ctx.SaveChangesAsync();

        var result = await svc.GetSeasonsBySeries(series.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value![0].Season.Should().Be("2024");
        result.Value[0].DriversChampId.Should().Be(dc.Id);
        result.Value[0].ConstructorsChampId.Should().Be(cc.Id);
    }

    // ─────────────────────────────────────────────
    // CreateChampionship
    // ─────────────────────────────────────────────

    [Fact]
    public async Task CreateChampionship_ShouldPersistBothChampionships()
    {
        var (ctx, svc) = BuildSut();

        var series = CreateSeries();
        await ctx.Series.AddAsync(series);
        await ctx.SaveChangesAsync();

        var dto = new ChampionshipCreateDto(series.Id, "2025", "WDC 2025", "WCC 2025");

        var result = await svc.CreateChampionship(dto);

        result.IsSuccess.Should().BeTrue();
        ctx.ChangeTracker.Clear();
        ctx.DriversChampionships.Should().HaveCount(1);
        ctx.ConstructorsChampionships.Should().HaveCount(1);
        ctx.DriversChampionships.First().Season.Should().Be("2025");
        ctx.DriversChampionships.First().Status.Should().Be("Upcoming");
    }

    // ─────────────────────────────────────────────
    // UpdateChampionshipStatus
    // ─────────────────────────────────────────────

    [Fact]
    public async Task UpdateChampionshipStatus_ShouldSucceed_AndPersistNewStatus()
    {
        var (ctx, svc) = BuildSut();

        var series = CreateSeries();
        await ctx.Series.AddAsync(series);

        var dc = CreateDriversChamp(series.Id, "2024");
        var cc = CreateConstructorsChamp(series.Id, "2024");
        await ctx.DriversChampionships.AddAsync(dc);
        await ctx.ConstructorsChampionships.AddAsync(cc);
        await ctx.SaveChangesAsync();

        var result = await svc.UpdateChampionshipStatus(dc.Id, cc.Id, "Finished");

        result.IsSuccess.Should().BeTrue();
        ctx.ChangeTracker.Clear();
        ctx.DriversChampionships.First().Status.Should().Be("Finished");
        ctx.ConstructorsChampionships.First().Status.Should().Be("Finished");
    }

    [Fact]
    public async Task UpdateChampionshipStatus_ShouldFail_WhenDriversChampNotFound()
    {
        var (_, svc) = BuildSut();

        var result = await svc.UpdateChampionshipStatus(Guid.NewGuid(), Guid.NewGuid(), "Finished");

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Egyéni bajnokság nem található");
    }

    // ─────────────────────────────────────────────
    // CreateParticipations
    // ─────────────────────────────────────────────

    [Fact]
    public async Task CreateParticipations_ShouldPersistDriverAndConstructor()
    {
        var (ctx, svc) = BuildSut();

        var series  = CreateSeries();
        var brand   = CreateBrand();
        await ctx.Series.AddAsync(series);
        await ctx.Brands.AddAsync(brand);

        var constructor = CreateConstructor(brand.Id);
        var driver      = CreateDriver();
        var dc          = CreateDriversChamp(series.Id, "2024");
        var cc          = CreateConstructorsChamp(series.Id, "2024");

        await ctx.Constructors.AddAsync(constructor);
        await ctx.Drivers.AddAsync(driver);
        await ctx.DriversChampionships.AddAsync(dc);
        await ctx.ConstructorsChampionships.AddAsync(cc);
        await ctx.SaveChangesAsync();

        var driverRow = new TestDriverParticipationRowDto(driver.Id, 44);
        var dto = new ParticipationAddDto(
            dc.Id,
            cc.Id,
            new List<DriverParticipationRowDto> { driverRow },
            new List<Guid> { constructor.Id }
        );

        var result = await svc.CreateParticipations(dto);

        result.IsSuccess.Should().BeTrue();
        ctx.ChangeTracker.Clear();
        ctx.DriverParticipates.Should().HaveCount(1);
        ctx.ConstructorCompetitions.Should().HaveCount(1);
    }

    [Fact]
    public async Task CreateParticipations_ShouldNotDuplicate_WhenAlreadyExists()
    {
        var (ctx, svc) = BuildSut();

        var series  = CreateSeries();
        var brand   = CreateBrand();
        await ctx.Series.AddAsync(series);
        await ctx.Brands.AddAsync(brand);

        var constructor = CreateConstructor(brand.Id);
        var dc          = CreateDriversChamp(series.Id, "2024");
        var cc          = CreateConstructorsChamp(series.Id, "2024");

        await ctx.Constructors.AddAsync(constructor);
        await ctx.DriversChampionships.AddAsync(dc);
        await ctx.ConstructorsChampionships.AddAsync(cc);

        // Előre hozzáadva
        var existingComp = new ConstructorCompetition
        {
            ConstructorId               = constructor.Id,
            ConstChampId                = cc.Id,
            ConstructorNameSnapshot     = constructor.Name,
            ConstructorNicknameSnapshot = constructor.Nickname
        };
        await ctx.ConstructorCompetitions.AddAsync(existingComp);
        await ctx.SaveChangesAsync();

        var dto = new ParticipationAddDto(
            dc.Id, cc.Id,
            new List<DriverParticipationRowDto>(),
            new List<Guid> { constructor.Id }
        );

        var result = await svc.CreateParticipations(dto);

        result.IsSuccess.Should().BeTrue();
        ctx.ChangeTracker.Clear();
        ctx.ConstructorCompetitions.Should().HaveCount(1); // Nem duplázódott
    }

    [Fact]
    public async Task CreateParticipations_ShouldFail_WhenConstructorNotFound()
    {
        var (ctx, svc) = BuildSut();

        var series = CreateSeries();
        var dc     = CreateDriversChamp(series.Id, "2024");
        var cc     = CreateConstructorsChamp(series.Id, "2024");
        await ctx.Series.AddAsync(series);
        await ctx.DriversChampionships.AddAsync(dc);
        await ctx.ConstructorsChampionships.AddAsync(cc);
        await ctx.SaveChangesAsync();

        var dto = new ParticipationAddDto(
            dc.Id, cc.Id,
            new List<DriverParticipationRowDto>(),
            new List<Guid> { Guid.NewGuid() }
        );

        var result = await svc.CreateParticipations(dto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Constructor dosen't exist");
    }

    // ─────────────────────────────────────────────
    // DeleteDriverParticipation
    // ─────────────────────────────────────────────

    [Fact]
    public async Task DeleteDriverParticipation_ShouldRemoveFromDb()
    {
        var (ctx, svc) = BuildSut();

        var series = CreateSeries();
        var driver = CreateDriver();
        var dc     = CreateDriversChamp(series.Id, "2024");
        await ctx.Series.AddAsync(series);
        await ctx.Drivers.AddAsync(driver);
        await ctx.DriversChampionships.AddAsync(dc);

        var participation = new DriverParticipation
        {
            DriverId            = driver.Id,
            DriverChampId       = dc.Id,
            DriverNumber        = 44,
            DriverNameSnapshot  = driver.Name
        };
        await ctx.DriverParticipates.AddAsync(participation);
        await ctx.SaveChangesAsync();

        var result = await svc.DeleteDriverParticipation(driver.Id, dc.Id);

        result.IsSuccess.Should().BeTrue();
        ctx.ChangeTracker.Clear();
        ctx.DriverParticipates.Should().BeEmpty();
    }

    // ─────────────────────────────────────────────
    // DeleteConstructorCompetition
    // ─────────────────────────────────────────────

    [Fact]
    public async Task DeleteConstructorCompetition_ShouldRemoveFromDb()
    {
        var (ctx, svc) = BuildSut();

        var series  = CreateSeries();
        var brand   = CreateBrand();
        await ctx.Series.AddAsync(series);
        await ctx.Brands.AddAsync(brand);

        var constructor = CreateConstructor(brand.Id);
        var cc          = CreateConstructorsChamp(series.Id, "2024");
        await ctx.Constructors.AddAsync(constructor);
        await ctx.ConstructorsChampionships.AddAsync(cc);

        var competition = new ConstructorCompetition
        {
            ConstructorId               = constructor.Id,
            ConstChampId                = cc.Id,
            ConstructorNameSnapshot     = constructor.Name,
            ConstructorNicknameSnapshot = constructor.Nickname
        };
        await ctx.ConstructorCompetitions.AddAsync(competition);
        await ctx.SaveChangesAsync();

        var result = await svc.DeleteConstructorCompetition(constructor.Id, cc.Id);

        result.IsSuccess.Should().BeTrue();
        ctx.ChangeTracker.Clear();
        ctx.ConstructorCompetitions.Should().BeEmpty();
    }

    // ─────────────────────────────────────────────
    // GetGrandsPrixByChampionship
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetGrandsPrixByChampionship_ShouldFail_WhenChampionshipNotFound()
    {
        var (_, svc) = BuildSut();

        var result = await svc.GetGrandsPrixByChampionship(Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Bajnokság nem található");
    }

    [Fact]
    public async Task GetGrandsPrixByChampionship_ShouldReturnGrandsPrix_OrderedByRoundNumber()
    {
        var (ctx, svc) = BuildSut();

        var series  = CreateSeries();
        var circuit = CreateCircuit();
        await ctx.Series.AddAsync(series);
        await ctx.Circuits.AddAsync(circuit);

        var dc = CreateDriversChamp(series.Id, "2024");
        await ctx.DriversChampionships.AddAsync(dc);

        var gp1 = CreateGrandPrix(series.Id, 2024, 2, circuit.Id);
        var gp2 = CreateGrandPrix(series.Id, 2024, 1, circuit.Id);
        await ctx.GrandsPrix.AddRangeAsync(gp1, gp2);
        await ctx.SaveChangesAsync();

        var result = await svc.GetGrandsPrixByChampionship(dc.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value![0].RoundNumber.Should().Be(1);
        result.Value[1].RoundNumber.Should().Be(2);
    }

    // ─────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────

    private static Series CreateSeries() => new()
    {
        Id = Guid.NewGuid(),
        Name = "Formula 1",
        ShortName = "F1",
        Description = "Random Desc",
        GoverningBody = "Governing",
        PointSystem = "Point System"
    };

    private static Brand CreateBrand() => new()
    {
        Id = Guid.NewGuid(),
        Name = "Test Brand",
        Description = "Random Description",
        Principal = "Random Principal",
        HeadQuarters = "Random Headquarters"
    };

    private static Constructor CreateConstructor(Guid brandId) => new()
    {
        Id = Guid.NewGuid(),
        BrandId = brandId,
        Name = "Red Bull Racing",
        Nickname = "RBR",
        HeadQuarters = "Random Headquarters",
        TeamChief = "Random Team Chief",
        TechnicalChief = "Random  Technical Chief",

    };

    private static Driver CreateDriver() => new()
    {
        Id          = Guid.NewGuid(),
        Name        = "Max Verstappen",
        Nationality = "Dutch",
        BirthDate   = new DateTime(1997, 9, 30)
    };

    private static DriversChampionship CreateDriversChamp(Guid seriesId, string season, string name = "WDC", Guid? id = null) => new()
    {
        Id       = id ?? Guid.NewGuid(),
        SeriesId = seriesId,
        Season   = season,
        Name     = name,
        Status   = "Upcoming"
    };

    private static ConstructorsChampionship CreateConstructorsChamp(Guid seriesId, string season, string name = "WCC", Guid? id = null) => new()
    {
        Id       = id ?? Guid.NewGuid(),
        SeriesId = seriesId,
        Season   = season,
        Name     = name,
        Status   = "Upcoming"
    };

    private static Circuit CreateCircuit() => new()
    {
        Id          = Guid.NewGuid(),
        Name        = "Monza",
        Location    = "Italy",
        Length      = 5.793,
        Type        = "Street",
        FastestLap  = "1:21.046"
    };

    private static GrandPrix CreateGrandPrix(Guid seriesId, int year, int round, Guid circuitId) => new()
    {
        Id            = Guid.NewGuid(),
        SeriesId      = seriesId,
        SeasonYear    = year,
        RoundNumber   = round,
        Name          = $"GP Round {round}",
        ShortName     = $"R{round}",
        CircuitId     = circuitId,
        StartTime     = DateTime.UtcNow,
        EndTime       = DateTime.UtcNow.AddHours(2),
        RaceDistance  = 305,
        LapsCompleted = 58
    };

    private sealed record TestDriverParticipationRowDto(Guid DriverId, int DriverNumber)
        : DriverParticipationRowDto(DriverId, DriverNumber);
}
