using Entities.Models.Standings;

namespace Repositories.Interfaces.Standings;

public interface IBrandsRepository
{
    Brand? GetBrandById(Guid id);
    List<Brand> GetAllBrands();
    void Create(Brand brand);
    void Update(Brand brand);
    void Delete(Guid id);
    Brand? GetByName(string name);
    bool CheckIfIdExists(Guid id);
}