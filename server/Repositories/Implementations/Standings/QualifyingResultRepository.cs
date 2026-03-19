using Context;
using Entities.Models.Standings;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.Standings;

namespace Repositories.Implementations.Standings;

public class QualifyingResultRepository(EfContext context) : IQualifyingResultRepository 
{
    public async Task<QualifyingResult?> GetByResultId(Guid resultId)
    {
        return await context.QualifyingResults
            .FirstOrDefaultAsync(qr => qr.ResultId == resultId);
    }

    public async Task<List<QualifyingResult>> GetByResultIds(List<Guid> resultIds)
    {
        return await context.QualifyingResults
            .Where(qr => resultIds.Contains(qr.ResultId))
            .ToListAsync();
    }

    public async Task Create(QualifyingResult qualifyingResult)
    {
        await context.QualifyingResults.AddAsync(qualifyingResult);
        await context.SaveChangesAsync();
    }

    public async Task Update(QualifyingResult qualifyingResult)
    {
        context.QualifyingResults.Update(qualifyingResult);
        await context.SaveChangesAsync();
    }
}