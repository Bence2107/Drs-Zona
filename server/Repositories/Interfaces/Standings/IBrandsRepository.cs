using Entities.Models.Standings;

namespace Repositories.Interfaces.Standings;

public interface IBrandsRepository
{
    Brand? GetBrandById(int id);
    List<Brand> GetAllBrands();
    void Create(Brand brand);
    void Update(Brand brand);
    void Delete(int id);
    Brand? GetByName(string name);
    bool CheckIfIdExists(int id);
}