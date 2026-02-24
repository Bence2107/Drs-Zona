using Entities.Models.Standings;

namespace Repositories.Interfaces.Standings;

public interface IBrandsRepository
{
    Task<Brand?> GetBrandById(Guid id);
    Task<List<Brand>> GetAllBrands();
    Task Create(Brand brand);
    Task Update(Brand brand);
    Task Delete(Guid id);
    Task<Brand?> GetByName(string name);
    Task<bool> CheckIfIdExists(Guid id);
}