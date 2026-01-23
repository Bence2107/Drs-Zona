using DTOs.Standings;

namespace Services.Interfaces;

public interface IBrandService
{
    ResponseResult<BrandDetailDto> GetBrandById(int id);
    ResponseResult<BrandDetailDto> GetBrandByName(string name);
    ResponseResult<List<BrandListDto>> ListBrands();
    ResponseResult<bool> CreateBrand(BrandCreateDto dto);
    ResponseResult<bool> UpdateBrand(BrandUpdateDto dto);
    ResponseResult<bool> DeleteBrand(int id);
}