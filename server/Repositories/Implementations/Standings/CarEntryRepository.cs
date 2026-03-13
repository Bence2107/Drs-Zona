using Context;
using Entities.Models.Standings;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.Standings;

namespace Repositories.Implementations.Standings;

public class CarEntryRepository(EfContext context) : ICarEntryRepository
{
    private readonly DbSet<CarEntry> _carEntries = context.CarEntries;

    
    public async Task<List<CarEntry>> GetByResultId(Guid resultId) => await  _carEntries
        .Where(r => r.ResultId == resultId)
        .ToListAsync();
    
    public async Task Create(CarEntry entry)
    {
        _carEntries.Add(entry);
        await context.SaveChangesAsync();
    }

    public async Task DeleteByResultId(Guid resultId)
    {
         var entries = await GetByResultId(resultId);
         if (entries.Count == 0) return;
         
         _carEntries.RemoveRange(entries);
         await context.SaveChangesAsync();
    }
}