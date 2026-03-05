using DTOs.Standings;

namespace Services.Interfaces;

public interface IConstructorsService
{
    Task<ResponseResult<ConstructorDetailDto>> GetById(Guid id);
    Task<ResponseResult<List<ConstructorListDto>>> GetAllConstructors();
    Task<ResponseResult<List<ConstructorListDto>>> ListAllConstructorsByChampionship(Guid championshipId);
    Task<ResponseResult<bool>> CreateConstructor(ConstructorCreateDto constructorCreateDto);
    Task<ResponseResult<bool>> UpdateConstructor(ConstructorUpdateDto constructorCreateDto);
}