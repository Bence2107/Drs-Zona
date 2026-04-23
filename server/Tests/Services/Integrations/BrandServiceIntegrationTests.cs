using Context;
using Entities.Models.Standings;
using FluentAssertions;
using Repositories.Implementations.Standings;
using Services.Implementations;
using Services.Interfaces;
using Xunit;

namespace Tests.Services.Integrations;

public class BrandIntegrationTests
{

    private static (EfContext ctx, IBrandService svc) BuildSut()
    {
        var ctx = InMemoryDbFactory.CreateContext();
        var svc = new BrandService(new BrandsRepository(ctx));
        return (ctx, svc);
    }

    // ─────────────────────────────────────────────
    // ListBrands
    // ─────────────────────────────────────────────

    [Fact]
    public async Task ListBrands_ShouldReturnAll_OrderedByName()
    {
        var (ctx, svc) = BuildSut();

        await ctx.Brands.AddRangeAsync(
            new Brand
            {
                Id = Guid.NewGuid(),
                Name = "Red Bull",
                Description = "null",
                Principal = "null",
                HeadQuarters = "null"
            },
            new Brand
            {
                Id = Guid.NewGuid(),
                Name = "Alpine",
                Description = "null",
                Principal = "null",
                HeadQuarters = "null"
            },
            new Brand
            {
                Id = Guid.NewGuid(),
                Name = "Mercedes",
                Description = "null",
                Principal = "null",
                HeadQuarters = "null"
            }
        );
        await ctx.SaveChangesAsync();

        var result = await svc.ListBrands();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);
        result.Value![0].Name.Should().Be("Alpine");
        result.Value[1].Name.Should().Be("Mercedes");
        result.Value[2].Name.Should().Be("Red Bull");
    }

    [Fact]
    public async Task ListBrands_ShouldReturnEmpty_WhenNoBrandsExist()
    {
        var (_, svc) = BuildSut();

        var result = await svc.ListBrands();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task ListBrands_ShouldPersistAndReturnCorrectIdAndName()
    {
        var (ctx, svc) = BuildSut();

        var brandId = Guid.NewGuid();
        await ctx.Brands.AddAsync(new Brand
        {
            Id = brandId,
            Name = "Ferrari",
            Description = "null",
            Principal = "null",
            HeadQuarters = "null"
        });
        await ctx.SaveChangesAsync();

        var result = await svc.ListBrands();

        result.IsSuccess.Should().BeTrue();
        result.Value![0].Id.Should().Be(brandId);
        result.Value[0].Name.Should().Be("Ferrari");
    }
}