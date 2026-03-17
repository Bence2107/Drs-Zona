using DTOs.Standings;
using Entities.Models.Standings;
using Repositories.Interfaces.Standings;
using Services.Interfaces;
using Services.Types;

namespace Services.Implementations;

public class BrandService(IBrandsRepository brandsRepo) : IBrandService
{
    public async Task<ResponseResult<List<BrandListDto>>> ListBrands()
    {
        var brands = (await brandsRepo.GetAllBrands()).Select(b => new BrandListDto(
            Id: b.Id,
            Name: b.Name)
        )
            .OrderBy(b => b.Name)
            .ToList();
        return ResponseResult<List<BrandListDto>>.Success(brands);
    }
}