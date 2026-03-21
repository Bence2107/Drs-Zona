using Context;
using DTOs.Standings;
using Entities.Models.Standings;
using FluentAssertions;
using Repositories.Implementations.Standings;
using Services.Implementations;
using Services.Interfaces;
using Xunit;

namespace Tests.Services.Integrations;

public class ConstructorsIntegrationTests
{
    private static (EfContext ctx, IConstructorsService svc) BuildSut()
    {
        var ctx = InMemoryDbFactory.CreateContext();
        var svc = new ConstructorsService(
            new ConstructorsRepository(ctx),
            new BrandsRepository(ctx),
            new ContractsRepository(ctx)
        );
        return (ctx, svc);
    }

    // ─────────────────────────────────────────────
    // GetById
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetById_ShouldFail_WhenConstructorNotFound()
    {
        var (_, svc) = BuildSut();

        var result = await svc.GetById(Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Constructor not found");
    }

    [Fact]
    public async Task GetById_ShouldSucceed_WithBrandAndNoDrivers()
    {
        var (ctx, svc) = BuildSut();

        var brand = CreateBrand();
        var constructor = CreateConstructor(brand.Id, brand);
        await ctx.Brands.AddAsync(brand);
        await ctx.Constructors.AddAsync(constructor);
        await ctx.SaveChangesAsync();

        var result = await svc.GetById(constructor.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(constructor.Id);
        result.Value.BrandName.Should().Be(brand.Name);
        result.Value.DriverNames.Should().BeEmpty();
    }

    [Fact]
    public async Task GetById_ShouldIncludeDrivers_WhenContractsExist()
    {
        var (ctx, svc) = BuildSut();

        var brand = CreateBrand();
        var constructor = CreateConstructor(brand.Id, brand);
        var driver = CreateDriver();
        var contract = new Contract { Id = Guid.NewGuid(), DriverId = driver.Id, ConstructorId = constructor.Id };

        await ctx.Brands.AddAsync(brand);
        await ctx.Constructors.AddAsync(constructor);
        await ctx.Drivers.AddAsync(driver);
        await ctx.Contracts.AddAsync(contract);
        await ctx.SaveChangesAsync();

        var result = await svc.GetById(constructor.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value!.DriverNames.Should().HaveCount(1);
        result.Value.DriverNames![0].Name.Should().Be(driver.Name);
    }

    // ─────────────────────────────────────────────
    // GetAllConstructors
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetAllConstructors_ShouldReturnAll_OrderedByName()
    {
        var (ctx, svc) = BuildSut();

        await ctx.Constructors.AddRangeAsync(
            CreateConstructor(name: "Red Bull"),
            CreateConstructor(name: "Alpine"),
            CreateConstructor(name: "Mercedes")
        );
        await ctx.SaveChangesAsync();

        var result = await svc.GetAllConstructors();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);
        result.Value![0].Name.Should().Be("Alpine");
        result.Value[2].Name.Should().Be("Red Bull");
    }

    // ─────────────────────────────────────────────
    // Create
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Create_ShouldSucceed_AndPersistConstructor()
    {
        var (ctx, svc) = BuildSut();

        var brand = CreateBrand();
        await ctx.Brands.AddAsync(brand);
        await ctx.SaveChangesAsync();

        var dto = new ConstructorCreateDto(
            brand.Id, "New Team", "NT", 2010,
            "London", "John Chief", "Jane Tech",
            3, 50, 120, 14
        );

        var result = await svc.Create(dto);

        result.IsSuccess.Should().BeTrue();
        ctx.ChangeTracker.Clear();
        ctx.Constructors.Should().HaveCount(1);
        ctx.Constructors.First().Name.Should().Be("New Team");
        ctx.Constructors.First().BrandId.Should().Be(brand.Id);
    }

    [Fact]
    public async Task Create_ShouldFail_WhenNameAlreadyExists()
    {
        var (ctx, svc) = BuildSut();

        var brand = CreateBrand();
        var constructor = CreateConstructor(brand.Id, name: "Existing Team");
        await ctx.Brands.AddAsync(brand);
        await ctx.Constructors.AddAsync(constructor);
        await ctx.SaveChangesAsync();

        var dto = new ConstructorCreateDto(
            brand.Id, "Existing Team", "ET", 2010,
            "London", "John Chief", "Jane Tech",
            0, 0, 0, 1
        );

        var result = await svc.Create(dto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Constructor already is in the Championship");
    }

    // ─────────────────────────────────────────────
    // Update
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Update_ShouldSucceed_AndPersistChanges()
    {
        var (ctx, svc) = BuildSut();

        var brand = CreateBrand();
        var constructor = CreateConstructor(brand.Id, brand);
        await ctx.Brands.AddAsync(brand);
        await ctx.Constructors.AddAsync(constructor);
        await ctx.SaveChangesAsync();

        var dto = new ConstructorUpdateDto(
            constructor.Id, brand.Id,
            "Updated Name", "UPD", 2005,
            "New HQ", "New Chief", "New Tech",
            7, 120, 250, 20
        );

        var result = await svc.Update(dto);

        result.IsSuccess.Should().BeTrue();
        ctx.ChangeTracker.Clear();
        var updated = await ctx.Constructors.FindAsync(constructor.Id);
        updated!.Name.Should().Be("Updated Name");
        updated.TeamChief.Should().Be("New Chief");
        updated.Wins.Should().Be(120);
    }

    [Fact]
    public async Task Update_ShouldFail_WhenConstructorNotFound()
    {
        var (_, svc) = BuildSut();

        var dto = new ConstructorUpdateDto(
            Guid.NewGuid(), Guid.NewGuid(),
            "Name", "NM", 2000,
            "HQ", "Chief", "Tech",
            0, 0, 0, 1
        );

        var result = await svc.Update(dto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Constructor not found");
    }

    [Fact]
    public async Task Update_ShouldFail_WhenNewBrandNotFound()
    {
        var (ctx, svc) = BuildSut();

        var brand = CreateBrand();
        var constructor = CreateConstructor(brand.Id, brand);
        await ctx.Brands.AddAsync(brand);
        await ctx.Constructors.AddAsync(constructor);
        await ctx.SaveChangesAsync();

        var dto = new ConstructorUpdateDto(
            constructor.Id, Guid.NewGuid(), // ismeretlen brand
            "Name", "NM", 2000,
            "HQ", "Chief", "Tech",
            0, 0, 0, 1
        );

        var result = await svc.Update(dto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Brand not found");
    }

    // ─────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────

    private static Brand CreateBrand() => new()
    {
        Id = Guid.NewGuid(),
        Name = "Test Brand",
        Description = "Brand description",
        Principal = "null",
        HeadQuarters = "null"
    };

    private static Constructor CreateConstructor(
        Guid? brandId = null,
        Brand? brand = null,
        string name = "Red Bull Racing") => new()
    {
        Id = Guid.NewGuid(),
        BrandId = brandId ?? Guid.NewGuid(),
        Brand = brand,
        Name = name,
        Nickname = "RBR",
        FoundedYear = 2005,
        HeadQuarters = "Milton Keynes",
        TeamChief = "Christian Horner",
        TechnicalChief = "Adrian Newey",
        Championships = 6,
        Wins = 112,
        Podiums = 240,
        Seasons = 19
    };

    private static Driver CreateDriver() => new()
    {
        Id = Guid.NewGuid(),
        Name = "Max Verstappen",
        Nationality = "Dutch",
        BirthDate = new DateTime(1997, 9, 30)
    };
}