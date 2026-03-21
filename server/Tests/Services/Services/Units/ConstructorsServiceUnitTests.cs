using DTOs.Standings;
using Entities.Models.Standings;
using FluentAssertions;
using Moq;
using Repositories.Interfaces.Standings;
using Services.Implementations;
using Xunit;

namespace Tests.Services.Units;

public class ConstructorsServiceUnitTests
{

    private readonly Mock<IConstructorsRepository> _constructorRepo = new();
    private readonly Mock<IBrandsRepository>       _brandsRepo      = new();
    private readonly Mock<IContractsRepository>    _contractsRepo   = new();

    private readonly ConstructorsService _svc;

    public ConstructorsServiceUnitTests()
    {
        _svc = new ConstructorsService(
            _constructorRepo.Object,
            _brandsRepo.Object,
            _contractsRepo.Object
        );
    }

    // ─────────────────────────────────────────────
    // GetById
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetById_ShouldFail_WhenConstructorNotFound()
    {
        var id = Guid.NewGuid();
        _constructorRepo.Setup(r => r.CheckIfIdExists(id)).ReturnsAsync(false);

        var result = await _svc.GetById(id);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Constructor not found");
    }

    [Fact]
    public async Task GetById_ShouldFail_WhenBrandIsNull()
    {
        var id          = Guid.NewGuid();
        var constructor = CreateConstructor(id, brandId: Guid.NewGuid());
        constructor.Brand = null;

        _constructorRepo.Setup(r => r.CheckIfIdExists(id)).ReturnsAsync(true);
        _constructorRepo.Setup(r => r.GetByIdWithBrand(id)).ReturnsAsync(constructor);

        var result = await _svc.GetById(id);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Brand is invalid");
    }

    [Fact]
    public async Task GetById_ShouldFail_WhenBrandIdNotFoundInRepo()
    {
        var brandId     = Guid.NewGuid();
        var id          = Guid.NewGuid();
        var brand       = CreateBrand(brandId);
        var constructor = CreateConstructor(id, brandId, brand);

        _constructorRepo.Setup(r => r.CheckIfIdExists(id)).ReturnsAsync(true);
        _constructorRepo.Setup(r => r.GetByIdWithBrand(id)).ReturnsAsync(constructor);
        _brandsRepo.Setup(r => r.CheckIfIdExists(brandId)).ReturnsAsync(false);

        var result = await _svc.GetById(id);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Brand not found");
    }

    [Fact]
    public async Task GetById_ShouldSucceed_AndReturnCorrectDto()
    {
        var brandId     = Guid.NewGuid();
        var id          = Guid.NewGuid();
        var brand       = CreateBrand(brandId);
        var constructor = CreateConstructor(id, brandId, brand);

        _constructorRepo.Setup(r => r.CheckIfIdExists(id)).ReturnsAsync(true);
        _constructorRepo.Setup(r => r.GetByIdWithBrand(id)).ReturnsAsync(constructor);
        _brandsRepo.Setup(r => r.CheckIfIdExists(brandId)).ReturnsAsync(true);
        _contractsRepo.Setup(r => r.GetByTeamId(id)).ReturnsAsync(new List<Contract>());

        var result = await _svc.GetById(id);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(id);
        result.Value.BrandName.Should().Be(brand.Name);
        result.Value.Name.Should().Be(constructor.Name);
    }

    [Fact]
    public async Task GetById_ShouldIncludeDriverNames_WhenContractsExist()
    {
        var brandId     = Guid.NewGuid();
        var id          = Guid.NewGuid();
        var brand       = CreateBrand(brandId);
        var constructor = CreateConstructor(id, brandId, brand);
        var driverId    = Guid.NewGuid();

        var contracts = new List<Contract>
        {
            new() { DriverId = driverId, ConstructorId = id, Driver = new Driver
                {
                    Id = driverId,
                    Name = "Max Verstappen",
                    Nationality = "NED"
                }
            }
        };

        _constructorRepo.Setup(r => r.CheckIfIdExists(id)).ReturnsAsync(true);
        _constructorRepo.Setup(r => r.GetByIdWithBrand(id)).ReturnsAsync(constructor);
        _brandsRepo.Setup(r => r.CheckIfIdExists(brandId)).ReturnsAsync(true);
        _contractsRepo.Setup(r => r.GetByTeamId(id)).ReturnsAsync(contracts);

        var result = await _svc.GetById(id);

        result.IsSuccess.Should().BeTrue();
        result.Value!.DriverNames.Should().HaveCount(1);
        result.Value.DriverNames![0].Name.Should().Be("Max Verstappen");
    }

    // ─────────────────────────────────────────────
    // GetAllConstructors
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetAllConstructors_ShouldReturnList_OrderedByName()
    {
        var constructors = new List<Constructor>
        {
            CreateConstructor(name: "Red Bull"),
            CreateConstructor(name: "Alpine"),
            CreateConstructor(name: "Mercedes"),
        };

        _constructorRepo.Setup(r => r.GetAllConstructor()).ReturnsAsync(constructors);

        var result = await _svc.GetAllConstructors();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);
        result.Value![0].Name.Should().Be("Alpine");
        result.Value[1].Name.Should().Be("Mercedes");
        result.Value[2].Name.Should().Be("Red Bull");
    }

    [Fact]
    public async Task GetAllConstructors_ShouldReturnEmpty_WhenNoneExist()
    {
        _constructorRepo.Setup(r => r.GetAllConstructor()).ReturnsAsync(new List<Constructor>());

        var result = await _svc.GetAllConstructors();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    // ─────────────────────────────────────────────
    // Create
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Create_ShouldFail_WhenNameAlreadyExists()
    {
        var existing = CreateConstructor(name: "Red Bull");
        _constructorRepo.Setup(r => r.GetByName("Red Bull")).ReturnsAsync(existing);

        var dto = CreateConstructorCreateDto("Red Bull");

        var result = await _svc.Create(dto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Constructor already is in the Championship");
    }

    [Fact]
    public async Task Create_ShouldSucceed_AndCallRepoCreate()
    {
        var brandId = Guid.NewGuid();
        _constructorRepo.Setup(r => r.GetByName(It.IsAny<string>())).ReturnsAsync((Constructor?)null);
        _brandsRepo.Setup(r => r.CheckIfIdExists(brandId)).ReturnsAsync(true);
        _constructorRepo.Setup(r => r.Create(It.IsAny<Constructor>())).Returns(Task.CompletedTask);

        var dto = CreateConstructorCreateDto("New Team", brandId);

        var result = await _svc.Create(dto);

        result.IsSuccess.Should().BeTrue();
        _constructorRepo.Verify(r => r.Create(It.Is<Constructor>(c => c.Name == "New Team")), Times.Once);
    }

    [Fact]
    public async Task Create_ShouldSetBrandId_WhenBrandExists()
    {
        var brandId = Guid.NewGuid();
        Constructor? saved = null;

        _constructorRepo.Setup(r => r.GetByName(It.IsAny<string>())).ReturnsAsync((Constructor?)null);
        _brandsRepo.Setup(r => r.CheckIfIdExists(brandId)).ReturnsAsync(true);
        _constructorRepo
            .Setup(r => r.Create(It.IsAny<Constructor>()))
            .Callback<Constructor>(c => saved = c)
            .Returns(Task.CompletedTask);

        var dto = CreateConstructorCreateDto("New Team", brandId);

        await _svc.Create(dto);

        saved!.BrandId.Should().Be(brandId);
    }

    [Fact]
    public async Task Create_ShouldNotSetBrandId_WhenBrandNotFound()
    {
        var brandId = Guid.NewGuid();
        Constructor? saved = null;

        _constructorRepo.Setup(r => r.GetByName(It.IsAny<string>())).ReturnsAsync((Constructor?)null);
        _brandsRepo.Setup(r => r.CheckIfIdExists(brandId)).ReturnsAsync(false);
        _constructorRepo
            .Setup(r => r.Create(It.IsAny<Constructor>()))
            .Callback<Constructor>(c => saved = c)
            .Returns(Task.CompletedTask);

        var dto = CreateConstructorCreateDto("New Team", brandId);

        await _svc.Create(dto);

        saved!.BrandId.Should().Be(Guid.Empty);
    }

    // ─────────────────────────────────────────────
    // Update
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Update_ShouldFail_WhenConstructorNotFound()
    {
        _constructorRepo.Setup(r => r.GetConstructorById(It.IsAny<Guid>())).ReturnsAsync((Constructor?)null);

        var dto = CreateConstructorUpdateDto(Guid.NewGuid(), Guid.NewGuid());

        var result = await _svc.Update(dto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Constructor not found");
    }

    [Fact]
    public async Task Update_ShouldFail_WhenNewBrandNotFound()
    {
        var oldBrandId  = Guid.NewGuid();
        var newBrandId  = Guid.NewGuid();
        var constructor = CreateConstructor(brandId: oldBrandId);

        _constructorRepo.Setup(r => r.GetConstructorById(constructor.Id)).ReturnsAsync(constructor);
        _brandsRepo.Setup(r => r.CheckIfIdExists(newBrandId)).ReturnsAsync(false);

        var dto = CreateConstructorUpdateDto(constructor.Id, newBrandId);

        var result = await _svc.Update(dto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Brand not found");
    }

    [Fact]
    public async Task Update_ShouldSucceed_AndCallRepoUpdate()
    {
        var brandId     = Guid.NewGuid();
        var constructor = CreateConstructor(brandId: brandId);

        _constructorRepo.Setup(r => r.GetConstructorById(constructor.Id)).ReturnsAsync(constructor);
        _constructorRepo.Setup(r => r.Update(It.IsAny<Constructor>())).Returns(Task.CompletedTask);

        // Ugyanaz a brandId — nem fut a brand-check
        var dto = CreateConstructorUpdateDto(constructor.Id, brandId, "Updated Name");

        var result = await _svc.Update(dto);

        result.IsSuccess.Should().BeTrue();
        _constructorRepo.Verify(r => r.Update(It.Is<Constructor>(c => c.Name == "Updated Name")), Times.Once);
    }

    [Fact]
    public async Task Update_ShouldNotCheckBrand_WhenBrandIdUnchanged()
    {
        var brandId     = Guid.NewGuid();
        var constructor = CreateConstructor(brandId: brandId);

        _constructorRepo.Setup(r => r.GetConstructorById(constructor.Id)).ReturnsAsync(constructor);
        _constructorRepo.Setup(r => r.Update(It.IsAny<Constructor>())).Returns(Task.CompletedTask);

        var dto = CreateConstructorUpdateDto(constructor.Id, brandId);

        await _svc.Update(dto);

        _brandsRepo.Verify(r => r.CheckIfIdExists(It.IsAny<Guid>()), Times.Never);
    }

    // ─────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────

    private static Brand CreateBrand(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        Name = "Test Brand",
        Description = "Brand description",
        Principal = "null",
        HeadQuarters = "null"
    };

    private static Constructor CreateConstructor(
        Guid?   id      = null,
        Guid?   brandId = null,
        Brand?  brand   = null,
        string  name    = "Red Bull Racing") => new()
    {
        Id             = id      ?? Guid.NewGuid(),
        BrandId        = brandId ?? Guid.NewGuid(),
        Brand          = brand,
        Name           = name,
        Nickname       = "RBR",
        FoundedYear    = 2005,
        HeadQuarters   = "Milton Keynes",
        TeamChief      = "Christian Horner",
        TechnicalChief = "Adrian Newey",
        Championships  = 6,
        Wins           = 112,
        Podiums        = 240,
        Seasons        = 19
    };

    private static ConstructorCreateDto CreateConstructorCreateDto(string name, Guid? brandId = null) =>
        new(brandId ?? Guid.NewGuid(), name, "RBR", 2005, "Milton Keynes",
            "Christian Horner", "Adrian Newey", 6, 112, 240, 19);

    private static ConstructorUpdateDto CreateConstructorUpdateDto(Guid id, Guid brandId, string name = "Red Bull Racing") =>
        new(id, brandId, name, "RBR", 2005, "Milton Keynes",
            "Christian Horner", "Adrian Newey", 6, 112, 240, 19);
}
