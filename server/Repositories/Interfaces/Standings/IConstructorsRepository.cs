using Entities.Models.Standings;

namespace Repositories.Interfaces.Standings;

public interface IConstructorsRepository
{
    Constructor? GetConstructorById(int id);
    List<Constructor> GetAllConstructor();
    void Create(Constructor constructor);
    void Update(Constructor constructor);
    void Delete(int id);
    Constructor? GetByIdWithBrand(int id);
    List<Constructor> GetByBrandId(int brandId);
    Constructor? GetByName(string name);
    bool CheckIfIdExists(int id);
}