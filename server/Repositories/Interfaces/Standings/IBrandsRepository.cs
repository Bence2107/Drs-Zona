using Entities.Models.Standings;

namespace Repositories.Interfaces.Standings;

public interface IBrandsRepository
{
    Task<List<Brand>> GetAllBrands();
    Task<bool> CheckIfIdExists(Guid id);
}