using DTOs.Standings;

namespace Services.Interfaces;

public interface IBrandService
{
    Task<ResponseResult<BrandDetailDto>> GetBrandById(Guid id);
    Task<ResponseResult<BrandDetailDto>> GetBrandByName(string name);
    Task<ResponseResult<List<BrandListDto>>> ListBrands();
    Task<ResponseResult<bool>> CreateBrand(BrandCreateDto dto);
    Task<ResponseResult<bool>> UpdateBrand(BrandUpdateDto dto);
    Task<ResponseResult<bool>> DeleteBrand(Guid id);
}