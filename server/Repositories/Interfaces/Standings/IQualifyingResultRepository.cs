using Entities.Models.Standings;

namespace Repositories.Interfaces.Standings;

public interface IQualifyingResultRepository
{
    Task<QualifyingResult?> GetByResultId(Guid resultId);
    
    Task<List<QualifyingResult>> GetByResultIds(List<Guid> resultId);
    Task AddAsync(QualifyingResult qualifyingResult);
    Task UpdateAsync(QualifyingResult qualifyingResult);
}