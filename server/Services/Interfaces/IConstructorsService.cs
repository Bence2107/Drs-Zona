using DTOs.Standings;

namespace Services.Interfaces;

public interface IConstructorsService
{
    ResponseResult<ConstructorDetailDto> GetById(Guid id);
    
    ResponseResult<List<ConstructorListDto>> ListAllConstructorsByChampionship(Guid championshipId);
    
    ResponseResult<bool> CreateConstructor(ConstructorCreateDto constructorCreateDto);
    
    ResponseResult<bool> UpdateConstructor(ConstructorUpdateDto constructorCreateDto);
}