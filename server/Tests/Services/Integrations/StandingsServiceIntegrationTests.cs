using Context;
using Entities.Models.RaceTracks;
using Entities.Models.Standings;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Repositories.Implementations.RaceTracks;
using Repositories.Implementations.Standings;
using Services.Implementations;
using Services.Interfaces;
using Xunit;

namespace Tests.Services.Integrations;

public class StandingsServiceIntegrationTests
{
    private static (EfContext ctx, IStandingsService svc) BuildSut()
    {
        var ctx = InMemoryDbFactory.CreateContext();
        var svc = new StandingsService(
            new ConstructorsChampionshipsRepository(ctx),
            new ConstructorCompetitionRepository(ctx),
            new DriversChampionshipsRepository(ctx),
            new DriverParticipationRepository(ctx),
            new GrandsPrixRepository(ctx),
            new QualifyingResultRepository(ctx),
            new ResultsRepository(ctx),
            new SeriesRepository(ctx)
        );
        return (ctx, svc);
    }

    // ─────────────────────────────────────────────
    // GetAllSeries
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetAllSeries_ShouldReturnAll()
    {
        var (ctx, svc) = BuildSut();

        await ctx.Series.AddRangeAsync(
            CreateSeries(),
            CreateSeries("Formula 2", "F2")
        );
        await ctx.SaveChangesAsync();

        var result = await svc.GetAllSeries();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    // ─────────────────────────────────────────────
    // GetConstructorStandings
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetConstructorStandings_ShouldFail_WhenChampNotFound()
    {
        var (_, svc) = BuildSut();

        var result = await svc.GetConstructorStandings(Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("A konstruktőri bajnokság nem található");
    }

    [Fact]
    public async Task GetConstructorStandings_ShouldReturnEmpty_WhenNoResults()
    {
        var (ctx, svc) = BuildSut();

        var series = CreateSeries();
        var champ  = CreateConstructorsChamp(series.Id);
        await ctx.Series.AddAsync(series);
        await ctx.ConstructorsChampionships.AddAsync(champ);
        await ctx.SaveChangesAsync();

        var result = await svc.GetConstructorStandings(champ.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Results.Should().BeEmpty();
    }

    [Fact]
    public async Task GetConstructorStandings_ShouldSumPointsPerConstructor_AndOrderDescending()
    {
        var (ctx, svc) = BuildSut();

        var series   = CreateSeries();
        var driversChamp = CreateDriversChamp(series.Id);
        var constructorsChamp    = CreateConstructorsChamp(series.Id);
        var cons1    = CreateConstructor("Red Bull");
        var cons2    = CreateConstructor("Alpine");
        var gp       = CreateGrandPrix(series.Id, 2024, 1);

        await ctx.Series.AddAsync(series);
        await ctx.DriversChampionships.AddAsync(driversChamp);
        await ctx.ConstructorsChampionships.AddAsync(constructorsChamp);
        await ctx.Constructors.AddRangeAsync(cons1, cons2);
        await ctx.GrandsPrix.AddAsync(gp);
        await ctx.SaveChangesAsync();

        // Red Bull: 25+18=43, Alpine: 15
        await ctx.Results.AddRangeAsync(
            CreateResult(gpId: gp.Id, constructorId: cons1.Id, constructorPoints: 25, finishPosition: 1, consChampId: constructorsChamp.Id, consName: "Red Bull", consNick: "RBR"),
            CreateResult(gpId: gp.Id, constructorId: cons1.Id, constructorPoints: 18, finishPosition: 2, consChampId: constructorsChamp.Id, consName: "Red Bull", consNick: "RBR"),
            CreateResult(gpId: gp.Id, constructorId: cons2.Id, constructorPoints: 15, finishPosition: 3, consChampId: constructorsChamp.Id, consName: "Alpine",   consNick: "ALP")
        );
        await ctx.SaveChangesAsync();
        
        var count = await ctx.Results.CountAsync(r => r.ConsChampId == constructorsChamp.Id);
        count.Should().BeGreaterThan(0); 

        var result = await svc.GetConstructorStandings(constructorsChamp.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Results.Should().HaveCount(2, result.Message);
        result.Value.Results[0].ConstructorShortName.Should().Be("RBR");
        result.Value.Results[0].Points.Should().Be(43);
        result.Value.Results[1].Points.Should().Be(15);
    }

    // ─────────────────────────────────────────────
    // GetDriverStandings
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetDriverStandings_ShouldFail_WhenChampNotFound()
    {
        var (_, svc) = BuildSut();

        var result = await svc.GetDriverStandings(Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Az egyéni bajnokság nem található");
    }

    [Fact]
    public async Task GetDriverStandings_ShouldReturnEmpty_WhenNoResults()
    {
        var (ctx, svc) = BuildSut();

        var series = CreateSeries();
        var champ  = CreateDriversChamp(series.Id);
        await ctx.Series.AddAsync(series);
        await ctx.DriversChampionships.AddAsync(champ);
        await ctx.SaveChangesAsync();

        var result = await svc.GetDriverStandings(champ.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Results.Should().BeEmpty();
    }
    

    // ─────────────────────────────────────────────
    // GetGrandPrixResults
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetGrandPrixResults_ShouldFail_WhenGrandPrixNotFound()
    {
        var (_, svc) = BuildSut();

        var result = await svc.GetGrandPrixResults(Guid.NewGuid(), "Futam");

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("A nagydíj nem létezik");
    }

    [Fact]
    public async Task GetGrandPrixResults_ShouldReturnEmpty_WhenNoSessionResults()
    {
        var (ctx, svc) = BuildSut();

        var series = CreateSeries();
        var gp     = CreateGrandPrix(series.Id, 2024, 1);
        await ctx.Series.AddAsync(series);
        await ctx.GrandsPrix.AddAsync(gp);
        await ctx.SaveChangesAsync();

        var result = await svc.GetGrandPrixResults(gp.Id, "Futam");

        result.IsSuccess.Should().BeTrue();
        result.Value!.Results.Should().BeEmpty();
    }

    [Fact]
    public async Task GetGrandPrixResults_ShouldReturnOrderedByPosition()
    {
        var (ctx, svc) = BuildSut();

        var series = CreateSeries();
        var gp     = CreateGrandPrix(series.Id, 2024, 1);
        await ctx.Series.AddAsync(series);
        await ctx.GrandsPrix.AddAsync(gp);
        await ctx.SaveChangesAsync();

        await ctx.Results.AddRangeAsync(
            CreateResult(gp.Id, driverName: "Hamilton",   finishPosition: 2, session: "Futam", raceTime: 5100000),
            CreateResult(gp.Id, driverName: "Verstappen", finishPosition: 1, session: "Futam", raceTime: 4900000)
        );
        await ctx.SaveChangesAsync();

        var result = await svc.GetGrandPrixResults(gp.Id, "Futam");

        result.IsSuccess.Should().BeTrue();
        result.Value!.Results.Should().HaveCount(2);
        result.Value.Results[0].Position.Should().Be(1);
        result.Value.Results[0].DriverName.Should().Be("Verstappen");
    }

    // ─────────────────────────────────────────────
    // GetSessionsByGrandPrix
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetSessionsByGrandPrix_ShouldReturnDistinctSessions()
    {
        var (ctx, svc) = BuildSut();

        var series = CreateSeries();
        var gp     = CreateGrandPrix(series.Id, 2024, 1);
        await ctx.Series.AddAsync(series);
        await ctx.GrandsPrix.AddAsync(gp);
        await ctx.SaveChangesAsync();

        await ctx.Results.AddRangeAsync(
            CreateResult(gp.Id, session: "Futam",   finishPosition: 1),
            CreateResult(gp.Id, session: "Futam",   finishPosition: 2),
            CreateResult(gp.Id, session: "Időmérő", finishPosition: 1)
        );
        await ctx.SaveChangesAsync();

        var result = await svc.GetSessionsByGrandPrix(gp.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().Contain("Futam");
        result.Value.Should().Contain("Időmérő");
    }
    
    // ─────────────────────────────────────────────
    // RecalculateSession
    // ─────────────────────────────────────────────

    [Fact]
    public async Task RecalculateSession_ShouldFail_WhenGrandPrixNotFound()
    {
        var (_, svc) = BuildSut();

        var result = await svc.RecalculateSession(Guid.NewGuid(), "Futam");

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Nagydíj nem található");
    }

    [Fact]
    public async Task RecalculateSession_ShouldSucceed_AndRecalculateF1Points()
    {
        var (ctx, svc) = BuildSut();

        var series = CreateSeries(pointSystem: "F1");
        var gp     = CreateGrandPrix(series.Id, 2024, 1);
        await ctx.Series.AddAsync(series);
        await ctx.GrandsPrix.AddAsync(gp);
        await ctx.SaveChangesAsync();

        var r1 = CreateResult(gp.Id, finishPosition: 1, session: "Futam", driverPoints: 0); // rossz pont
        var r2 = CreateResult(gp.Id, finishPosition: 2, session: "Futam", driverPoints: 0);
        await ctx.Results.AddRangeAsync(r1, r2);
        await ctx.SaveChangesAsync();

        var result = await svc.RecalculateSession(gp.Id, "Futam");

        result.IsSuccess.Should().BeTrue();
        ctx.ChangeTracker.Clear();

        var updated1 = await ctx.Results.FindAsync(r1.Id);
        var updated2 = await ctx.Results.FindAsync(r2.Id);
        updated1!.DriverPoints.Should().Be(25);  // F1 P1 = 25
        updated2!.DriverPoints.Should().Be(18);  // F1 P2 = 18
    }

    // ─────────────────────────────────────────────
    // UpdateSingleResult
    // ─────────────────────────────────────────────

    [Fact]
    public async Task UpdateSingleResult_ShouldFail_WhenResultNotFound()
    {
        var (_, svc) = BuildSut();

        var dto = new DTOs.Standings.SingleResultUpdateDto(Guid.NewGuid(), 1, "1:30.000", 53, "Finished", false, false);

        var result = await svc.UpdateSingleResult(dto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Eredmény nem található");
    }

    [Fact]
    public async Task UpdateSingleResult_ShouldSucceed_AndPersistChanges()
    {
        var (ctx, svc) = BuildSut();

        var series = CreateSeries(pointSystem: "F1");
        var gp     = CreateGrandPrix(series.Id, 2024, 1);
        await ctx.Series.AddAsync(series);
        await ctx.GrandsPrix.AddAsync(gp);
        await ctx.SaveChangesAsync();

        var entity = CreateResult(gp.Id, finishPosition: 3, session: "Futam", driverPoints: 15);
        await ctx.Results.AddAsync(entity);
        await ctx.SaveChangesAsync();

        // Módosítjuk P1-re
        var dto = new DTOs.Standings.SingleResultUpdateDto(
            entity.Id, 1, "1:30:00.000", 53, "Finished", false, false
        );

        var result = await svc.UpdateSingleResult(dto);

        result.IsSuccess.Should().BeTrue();
        ctx.ChangeTracker.Clear();
        var updated = await ctx.Results.FindAsync(entity.Id);
        updated!.FinishPosition.Should().Be(1);
        updated.DriverPoints.Should().Be(25); // F1 P1 = 25
    }

    // ─────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────

    private static Series CreateSeries(
        string name        = "Formula 1",
        string shortName   = "F1",
        string pointSystem = "F1",
        int    lastYear    = 2024) => new()
    {
        Id            = Guid.NewGuid(),
        Name          = name,
        ShortName     = shortName,
        PointSystem   = pointSystem,
        GoverningBody = "FIA",
        Description   = "Top motorsport",
        FirstYear     = 1950,
        LastYear      = lastYear
    };

    private static ConstructorsChampionship CreateConstructorsChamp(Guid seriesId) => new()
    {
        Id       = Guid.NewGuid(),
        SeriesId = seriesId,
        Season   = "2024",
        Name     = "WCC",
        Status   = "Active"
    };

    private static DriversChampionship CreateDriversChamp(Guid seriesId) => new()
    {
        Id       = Guid.NewGuid(),
        SeriesId = seriesId,
        Season   = "2024",
        Name     = "WDC",
        Status   = "Active"
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

    private static GrandPrix CreateGrandPrix(Guid seriesId, int year, int round, string name = "Test GP") => new()
    {
        Id            = Guid.NewGuid(),
        SeriesId      = seriesId,
        Name          = name,
        ShortName     = name.Length >= 3 ? name[..3].ToUpper() : "TST",
        RoundNumber   = round,
        SeasonYear    = year,
        StartTime     = DateTime.UtcNow,
        EndTime       = DateTime.UtcNow.AddHours(2),
        RaceDistance  = 306,
        LapsCompleted = 53
    };

    private static Result CreateResult(
        Guid? gpId           = null,
        Guid? driverId       = null,
        Guid? constructorId  = null,
        Guid? driverChampId  = null,
        Guid? consChampId    = null,
        string driverName    = "Driver",
        string consName      = "Team",
        string consNick      = "TM",
        int finishPosition   = 1,
        double driverPoints  = 0,
        double constructorPoints = 0,
        string session       = "Futam",
        long raceTime        = 5000000,
        GrandPrix? gp        = null) => new()   // ← gp navigáció külön
    {
        Id                          = Guid.NewGuid(),
        GrandPrixId                 = gp?.Id ?? gpId ?? Guid.NewGuid(),
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
