using DTOs.Standings;
using Services.Types;

namespace Services.Interfaces;

public interface IBrandService 
{
    Task<ResponseResult<List<BrandListDto>>> ListBrands();
}