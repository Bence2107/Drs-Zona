using Entities.Models.Standings;
using FluentAssertions;
using Moq;
using Repositories.Interfaces.Standings;
using Services.Implementations;
using Xunit;

namespace Tests.Services.Units;

public class BrandServiceUnitTests
{

    private readonly Mock<IBrandsRepository> _brandsRepo = new();
    private readonly BrandService            _svc;

    public BrandServiceUnitTests()
    {
        _svc    = new BrandService(_brandsRepo.Object);
    }

    // ─────────────────────────────────────────────
    // ListBrands
    // ─────────────────────────────────────────────

    [Fact]
    public async Task ListBrands_ShouldReturnList_OrderedByName()
    {
        _brandsRepo.Setup(r => r.GetAllBrands()).ReturnsAsync(new List<Brand>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Red Bull",
                Description = "null",
                Principal = "null",
                HeadQuarters = "null"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Alpine",
                Description = "null",
                Principal = "null",
                HeadQuarters = "null"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Mercedes",
                Description = "null",
                Principal = "null",
                HeadQuarters = "null"
            },
        });

        var result = await _svc.ListBrands();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);
        result.Value![0].Name.Should().Be("Alpine");
        result.Value[1].Name.Should().Be("Mercedes");
        result.Value[2].Name.Should().Be("Red Bull");
    }

    [Fact]
    public async Task ListBrands_ShouldReturnEmpty_WhenNoBrandsExist()
    {
        _brandsRepo.Setup(r => r.GetAllBrands()).ReturnsAsync(new List<Brand>());

        var result = await _svc.ListBrands();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task ListBrands_ShouldMapIdAndName()
    {
        var brandId = Guid.NewGuid();
        _brandsRepo.Setup(r => r.GetAllBrands()).ReturnsAsync(new List<Brand>
        {
            new()
            {
                Id = brandId,
                Name = "Ferrari",
                Description = "null",
                Principal = "null",
                HeadQuarters = "null"
            }
        });

        var result = await _svc.ListBrands();

        result.IsSuccess.Should().BeTrue();
        result.Value![0].Id.Should().Be(brandId);
        result.Value[0].Name.Should().Be("Ferrari");
    }
}
