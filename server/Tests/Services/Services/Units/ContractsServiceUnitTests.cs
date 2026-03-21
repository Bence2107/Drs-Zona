using DTOs.Standings;
using Entities.Models.Standings;
using FluentAssertions;
using Moq;
using Repositories.Interfaces.Standings;
using Services.Implementations;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Services.Units;

public class ContractsServiceUnitTests
{
    private readonly ITestOutputHelper _output;

    private readonly Mock<IContractsRepository> _contractsRepo = new();
    private readonly ContractsService _svc;

    public ContractsServiceUnitTests(ITestOutputHelper output)
    {
        _output = output;
        _svc = new ContractsService(_contractsRepo.Object);
    }

    // ─────────────────────────────────────────────
    // GetAllContracts
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetAllContracts_ShouldReturnList_OrderedByTeamName()
    {
        var contracts = new List<Contract>
        {
            CreateContract("Max Verstappen", "Red Bull"),
            CreateContract("Lewis Hamilton", "Alpine"),
            CreateContract("Charles Leclerc", "Mercedes"),
        };

        _contractsRepo.Setup(r => r.GetAllWithAll()).ReturnsAsync(contracts);

        var result = await _svc.GetAllContracts();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);
        result.Value![0].TeamName.Should().Be("Alpine");
        result.Value[1].TeamName.Should().Be("Mercedes");
        result.Value[2].TeamName.Should().Be("Red Bull");
    }

    [Fact]
    public async Task GetAllContracts_ShouldUseFallback_WhenDriverOrConstructorIsNull()
    {
        var contract = new Contract
        {
            Id = Guid.NewGuid(),
            DriverId = Guid.NewGuid(),
            ConstructorId = Guid.NewGuid(),
            Driver = null,
            Constructor = null
        };

        _contractsRepo.Setup(r => r.GetAllWithAll()).ReturnsAsync(new List<Contract> { contract });

        var result = await _svc.GetAllContracts();

        result.IsSuccess.Should().BeTrue();
        result.Value![0].DriverName.Should().Be("Üres");
        result.Value[0].TeamName.Should().Be("Üres");
    }

    [Fact]
    public async Task GetAllContracts_ShouldReturnEmpty_WhenNoContracts()
    {
        _contractsRepo.Setup(r => r.GetAllWithAll()).ReturnsAsync(new List<Contract>());

        var result = await _svc.GetAllContracts();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    // ─────────────────────────────────────────────
    // Create
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Create_ShouldSucceed_AndCallRepoCreate()
    {
        var driverId = Guid.NewGuid();
        var teamId = Guid.NewGuid();

        _contractsRepo.Setup(r => r.Create(It.IsAny<Contract>())).Returns(Task.CompletedTask);

        var dto = new ContractCreateDto(driverId, teamId);
        var result = await _svc.Create(dto);

        result.IsSuccess.Should().BeTrue();
        _contractsRepo.Verify(r => r.Create(It.Is<Contract>(c =>
            c.DriverId == driverId &&
            c.ConstructorId == teamId
        )), Times.Once);
    }

    // ─────────────────────────────────────────────
    // Update
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Update_ShouldFail_WhenContractNotFound()
    {
        var id = Guid.NewGuid();
        _contractsRepo.Setup(r => r.GetContractById(id)).ReturnsAsync((Contract?)null);

        var result = await _svc.Update(id, Guid.NewGuid(), Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Contract not found");
    }

    [Fact]
    public async Task Update_ShouldSucceed_AndCallRepoUpdate()
    {
        var contract = CreateContract();
        var newDriver = Guid.NewGuid();
        var newTeam = Guid.NewGuid();

        _contractsRepo.Setup(r => r.GetContractById(contract.Id)).ReturnsAsync(contract);
        _contractsRepo.Setup(r => r.Update(It.IsAny<Contract>())).Returns(Task.CompletedTask);

        var result = await _svc.Update(contract.Id, newDriver, newTeam);

        result.IsSuccess.Should().BeTrue();
        _contractsRepo.Verify(r => r.Update(It.Is<Contract>(c =>
            c.DriverId == newDriver &&
            c.ConstructorId == newTeam
        )), Times.Once);
    }

    // ─────────────────────────────────────────────
    // Delete
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Delete_ShouldFail_WhenContractNotFound()
    {
        var id = Guid.NewGuid();
        _contractsRepo.Setup(r => r.CheckIfIdExists(id)).ReturnsAsync(false);

        var result = await _svc.Delete(id);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Contract not found");
    }

    [Fact]
    public async Task Delete_ShouldSucceed_AndCallRepoDelete()
    {
        var id = Guid.NewGuid();
        _contractsRepo.Setup(r => r.CheckIfIdExists(id)).ReturnsAsync(true);
        _contractsRepo.Setup(r => r.Delete(id)).Returns(Task.CompletedTask);

        var result = await _svc.Delete(id);

        result.IsSuccess.Should().BeTrue();
        _contractsRepo.Verify(r => r.Delete(id), Times.Once);
    }

    // ─────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────

    private static Contract CreateContract(
        string driverName = "Test Driver",
        string constructorName = "Test Team") => new()
    {
        Id = Guid.NewGuid(),
        DriverId = Guid.NewGuid(),
        ConstructorId = Guid.NewGuid(),
        Driver = new Driver
            { Id = Guid.NewGuid(), Name = driverName, Nationality = "HU", BirthDate = new DateTime(1990, 1, 1) },
        Constructor = new Constructor
        {
            Id = Guid.NewGuid(),
            Name = constructorName,
            Nickname = "TT",
            HeadQuarters = "null",
            TeamChief = "null",
            TechnicalChief = "null",
        }
    };
}