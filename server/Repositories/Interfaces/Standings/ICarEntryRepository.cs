using Entities.Models.Standings;

namespace Repositories.Interfaces.Standings;

public interface ICarEntryRepository
{
    Task<List<CarEntry>> GetByResultId(Guid resultId);
    Task Create(CarEntry entry);
    Task DeleteByResultId(Guid resultId);
}