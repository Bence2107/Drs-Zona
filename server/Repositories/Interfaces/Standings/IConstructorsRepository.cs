using Entities.Models.Standings;

namespace Repositories.Interfaces.Standings;

public interface IConstructorsRepository
{
    Constructor? GetConstructorById(Guid id);
    List<Constructor> GetAllConstructor();
    void Create(Constructor constructor);
    void Update(Constructor constructor);
    void Delete(Guid id);
    Constructor? GetByIdWithBrand(Guid id);
    List<Constructor> GetByBrandId(Guid brandId);
    Constructor? GetByName(string name);
    bool CheckIfIdExists(Guid id);
}