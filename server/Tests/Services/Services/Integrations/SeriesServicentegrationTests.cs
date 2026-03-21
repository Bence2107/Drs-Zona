using Context;
using Entities.Models.Standings;
using FluentAssertions;
using Repositories.Implementations.Standings;
using Services.Implementations;
using Services.Interfaces;
using Xunit;

namespace Tests.Services.Integrations;

public class SeriesSeriesIntegrationTests
{

    private static (EfContext ctx, ISeriesService svc) BuildSut()
    {
        var ctx = InMemoryDbFactory.CreateContext();
        var svc = new SeriesService(
            new SeriesRepository(ctx),
            new DriversChampionshipsRepository(ctx),
            new ConstructorsChampionshipsRepository(ctx)
        );
        return (ctx, svc);
    }

    // ─────────────────────────────────────────────
    // GetSeriesByName
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetSeriesByName_ShouldFail_WhenSeriesNotFound()
    {
        var (_, svc) = BuildSut();

        var result = await svc.GetSeriesByName("NONEXISTENT");

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Series not exist");
    }

    [Fact]
    public async Task GetSeriesByName_ShouldSucceed_AndReturnCorrectFields()
    {
        var (ctx, svc) = BuildSut();

        var series = CreateSeries();
        await ctx.Series.AddAsync(series);
        await ctx.SaveChangesAsync();

        var result = await svc.GetSeriesByName(series.ShortName);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(series.Id);
        result.Value.Name.Should().Be(series.Name);
        result.Value.ShortName.Should().Be(series.ShortName);
        result.Value.Description.Should().Be(series.Description);
        result.Value.GoverningBody.Should().Be(series.GoverningBody);
        result.Value.FirstYear.Should().Be(series.FirstYear);
        result.Value.LastYear.Should().Be(series.LastYear);
    }

    [Fact]
    public async Task GetSeriesByName_ShouldReturnAvailableSeasons_OrderedDescending()
    {
        var (ctx, svc) = BuildSut();

        var series = CreateSeries();
        await ctx.Series.AddAsync(series);

        await ctx.DriversChampionships.AddRangeAsync(
            CreateDriversChamp(series.Id, "2022"),
            CreateDriversChamp(series.Id, "2024")
        );
        await ctx.ConstructorsChampionships.AddRangeAsync(
            CreateConstructorsChamp(series.Id, "2022"),
            CreateConstructorsChamp(series.Id, "2024")
        );
        await ctx.SaveChangesAsync();

        var result = await svc.GetSeriesByName(series.ShortName);

        result.IsSuccess.Should().BeTrue();
        result.Value!.AvailableSeasons.Should().HaveCount(2);
        result.Value.AvailableSeasons![0].Should().Be("2024");
        result.Value.AvailableSeasons[1].Should().Be("2022");
    }

    [Fact]
    public async Task GetSeriesByName_ShouldDeduplicateSeasons_WhenBothChampsHaveSameSeason()
    {
        var (ctx, svc) = BuildSut();

        var series = CreateSeries();
        await ctx.Series.AddAsync(series);

        await ctx.DriversChampionships.AddAsync(CreateDriversChamp(series.Id, "2024"));
        await ctx.ConstructorsChampionships.AddAsync(CreateConstructorsChamp(series.Id, "2024"));
        await ctx.SaveChangesAsync();

        var result = await svc.GetSeriesByName(series.ShortName);

        result.IsSuccess.Should().BeTrue();
        result.Value!.AvailableSeasons.Should().HaveCount(1);
        result.Value.AvailableSeasons![0].Should().Be("2024");
    }

    [Fact]
    public async Task GetSeriesByName_ShouldReturnEmptySeasons_WhenNoChampionshipsExist()
    {
        var (ctx, svc) = BuildSut();

        var series = CreateSeries();
        await ctx.Series.AddAsync(series);
        await ctx.SaveChangesAsync();

        var result = await svc.GetSeriesByName(series.ShortName);

        result.IsSuccess.Should().BeTrue();
        result.Value!.AvailableSeasons.Should().BeEmpty();
    }

    // ─────────────────────────────────────────────
    // ListSeries
    // ─────────────────────────────────────────────

    [Fact]
    public async Task ListSeries_ShouldReturnAll_OrderedByShortName()
    {
        var (ctx, svc) = BuildSut();

        await ctx.Series.AddRangeAsync(
            CreateSeries("MotoGP",    "MOTOGP"),
            CreateSeries(),
            CreateSeries("Formula 2", "F2")
        );
        await ctx.SaveChangesAsync();

        var result = await svc.ListSeries();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);
        result.Value![0].ShortName.Should().Be("F1");
        result.Value[1].ShortName.Should().Be("F2");
        result.Value[2].ShortName.Should().Be("MOTOGP");
    }

    [Fact]
    public async Task ListSeries_ShouldReturnEmpty_WhenNoneExist()
    {
        var (_, svc) = BuildSut();

        var result = await svc.ListSeries();

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

