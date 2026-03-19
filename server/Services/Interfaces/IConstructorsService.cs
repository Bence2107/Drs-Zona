using DTOs.Standings;
using Services.Types;

namespace Services.Interfaces;

public interface IConstructorsService 
{
    Task<ResponseResult<ConstructorDetailDto>> GetById(Guid id);
    Task<ResponseResult<List<ConstructorListDto>>> GetAllConstructors();
    Task<ResponseResult<List<ConstructorListDto>>> ListAllConstructorsByChampionship(Guid championshipId);
    Task<ResponseResult<bool>> Create(ConstructorCreateDto constructorCreateDto);
    Task<ResponseResult<bool>> Update(ConstructorUpdateDto constructorCreateDto);
}