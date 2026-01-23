using DTOs.Standings;

namespace Services.Interfaces;

public interface IConstructorsService
{
    ResponseResult<ConstructorDetailDto> GetById(int id);
    
    ResponseResult<List<ConstructorListDto>> ListAllConstructorsByChampionship(int championshipId);
    
    ResponseResult<bool> CreateConstructor(ConstructorCreateDto constructorCreateDto);
    
    ResponseResult<bool> UpdateConstructor(ConstructorUpdateDto constructorCreateDto);
}