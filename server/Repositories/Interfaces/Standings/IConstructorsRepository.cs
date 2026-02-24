using Entities.Models.Standings;

namespace Repositories.Interfaces.Standings;

public interface IConstructorsRepository
{
    Task<Constructor?> GetConstructorById(Guid id);
    Task<List<Constructor>> GetAllConstructor();
    Task Create(Constructor constructor);
    Task Update(Constructor constructor);
    Task Delete(Guid id);
    Task<Constructor?> GetByIdWithBrand(Guid id);
    Task<List<Constructor>> GetByBrandId(Guid brandId);
    Task<Constructor?> GetByName(string name);
    Task<bool> CheckIfIdExists(Guid id);
}