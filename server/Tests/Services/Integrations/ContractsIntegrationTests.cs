using Context;
using DTOs.Standings;
using Entities.Models.Standings;
using FluentAssertions;
using Repositories.Implementations.Standings;
using Services.Implementations;
using Services.Interfaces;
using Xunit;

namespace Tests.Services.Integrations;

public class ContractsIntegrationTests
{
    private static (EfContext ctx, IContractsService svc) BuildSut()
    {
        var ctx = InMemoryDbFactory.CreateContext();
        var svc = new ContractsService(new ContractsRepository(ctx));
        return (ctx, svc);
    }

    // ─────────────────────────────────────────────
    // GetAllContracts
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetAllContracts_ShouldReturnAll_OrderedByTeamName()
    {
        var (ctx, svc) = BuildSut();

        var driver1 = CreateDriver("Lewis Hamilton");
        var driver2 = CreateDriver("Max Verstappen");
        var constructor1 = CreateConstructor("Red Bull");
        var constructor2 = CreateConstructor("Alpine");

        await ctx.Drivers.AddRangeAsync(driver1, driver2);
        await ctx.Constructors.AddRangeAsync(constructor1, constructor2);
        await ctx.SaveChangesAsync();

        await ctx.Contracts.AddRangeAsync(
            new Contract { Id = Guid.NewGuid(), DriverId = driver1.Id, ConstructorId = constructor1.Id },
            new Contract { Id = Guid.NewGuid(), DriverId = driver2.Id, ConstructorId = constructor2.Id }
        );
        await ctx.SaveChangesAsync();

        var result = await svc.GetAllContracts();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value![0].TeamName.Should().Be("Alpine");
        result.Value[1].TeamName.Should().Be("Red Bull");
    }

    [Fact]
    public async Task GetAllContracts_ShouldReturnEmpty_WhenNoContracts()
    {
        var (_, svc) = BuildSut();

        var result = await svc.GetAllContracts();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    // ─────────────────────────────────────────────
    // Create
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Create_ShouldPersistContract()
    {
        var (ctx, svc) = BuildSut();

        var driver = CreateDriver();
        var constructor = CreateConstructor();
        await ctx.Drivers.AddAsync(driver);
        await ctx.Constructors.AddAsync(constructor);
        await ctx.SaveChangesAsync();

        var dto = new ContractCreateDto(driver.Id, constructor.Id);
        var result = await svc.Create(dto);

        result.IsSuccess.Should().BeTrue();
        ctx.ChangeTracker.Clear();
        ctx.Contracts.Should().HaveCount(1);
        ctx.Contracts.First().DriverId.Should().Be(driver.Id);
        ctx.Contracts.First().ConstructorId.Should().Be(constructor.Id);
    }

    // ─────────────────────────────────────────────
    // Update
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Update_ShouldSucceed_AndPersistChanges()
    {
        var (ctx, svc) = BuildSut();

        var driver1 = CreateDriver("Old Driver");
        var driver2 = CreateDriver("New Driver");
        var constructor = CreateConstructor();
        await ctx.Drivers.AddRangeAsync(driver1, driver2);
        await ctx.Constructors.AddAsync(constructor);
        await ctx.SaveChangesAsync();

        var contract = new Contract { Id = Guid.NewGuid(), DriverId = driver1.Id, ConstructorId = constructor.Id };
        await ctx.Contracts.AddAsync(contract);
        await ctx.SaveChangesAsync();

        var result = await svc.Update(contract.Id, driver2.Id, constructor.Id);

        result.IsSuccess.Should().BeTrue();
        ctx.ChangeTracker.Clear();
        var updated = await ctx.Contracts.FindAsync(contract.Id);
        updated!.DriverId.Should().Be(driver2.Id);
    }

    [Fact]
    public async Task Update_ShouldFail_WhenContractNotFound()
    {
        var (_, svc) = BuildSut();

        var result = await svc.Update(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Contract not found");
    }

    // ─────────────────────────────────────────────
    // Delete
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Delete_ShouldSucceed_AndRemoveFromDb()
    {
        var (ctx, svc) = BuildSut();

        var contract = new Contract { Id = Guid.NewGuid(), DriverId = Guid.NewGuid(), ConstructorId = Guid.NewGuid() };
        await ctx.Contracts.AddAsync(contract);
        await ctx.SaveChangesAsync();

        var result = await svc.Delete(contract.Id);

        result.IsSuccess.Should().BeTrue();
        ctx.ChangeTracker.Clear();
        ctx.Contracts.Should().BeEmpty();
    }

    [Fact]
    public async Task Delete_ShouldFail_WhenContractNotFound()
    {
        var (_, svc) = BuildSut();

        var result = await svc.Delete(Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Contract not found");
    }

    [Fact]
    public async Task Delete_ShouldOnlyDeleteTargetContract()
    {
        var (ctx, svc) = BuildSut();

        var c1 = new Contract { Id = Guid.NewGuid(), DriverId = Guid.NewGuid(), ConstructorId = Guid.NewGuid() };
        var c2 = new Contract { Id = Guid.NewGuid(), DriverId = Guid.NewGuid(), ConstructorId = Guid.NewGuid() };
        await ctx.Contracts.AddRangeAsync(c1, c2);
        await ctx.SaveChangesAsync();

        await svc.Delete(c1.Id);

        ctx.ChangeTracker.Clear();
        ctx.Contracts.Should().HaveCount(1);
        ctx.Contracts.First().Id.Should().Be(c2.Id);
    }

    // ─────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────

    private static Driver CreateDriver(string name = "Test Driver") => new()
    {
        Id = Guid.NewGuid(),
        Name = name,
        Nationality = "HU",
        BirthDate = new DateTime(1990, 1, 1)
    };

    private static Constructor CreateConstructor(string name = "Test Team") => new()
    {
        Id = Guid.NewGuid(),
        Name = name,
        Nickname = "TT",
        HeadQuarters = "null",
        TeamChief = "null",
        TechnicalChief = "null"
    };
}