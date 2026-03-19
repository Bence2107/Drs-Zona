using Entities.Models.Standings;

namespace Repositories.Interfaces.Standings;

public interface IConstructorsRepository 
{
    Task<Constructor?> GetConstructorById(Guid id);
    Task<List<Constructor>> GetAllConstructor();
    Task<Constructor?> GetByIdWithBrand(Guid id);
    Task<Constructor?> GetByName(string name);
    Task Create(Constructor constructor);
    Task Update(Constructor constructor);
    Task<bool> CheckIfIdExists(Guid id);
}