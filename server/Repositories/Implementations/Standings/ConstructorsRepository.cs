using Context;
using Entities.Models.Standings;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.Standings;

namespace Repositories.Implementations.Standings;

public class ConstructorsRepository(EfContext context) : IConstructorsRepository
{
    private readonly DbSet<Constructor> _constructors = context.Constructors;
    
    public async Task<Constructor?> GetConstructorById(Guid id) => await _constructors
        .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<List<Constructor>> GetAllConstructor() => await _constructors.ToListAsync();

    public async Task Create(Constructor constructor)
    {
        _constructors.Add(constructor);
        await context.SaveChangesAsync();
    }

    public async Task Update(Constructor constructor)
    {
        _constructors.Update(constructor);
        await context.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        var  constructor = await GetConstructorById(id);
        if(constructor == null) return;
        
        _constructors.Remove(constructor);
        await context.SaveChangesAsync();
    }

    public async Task<Constructor?> GetByIdWithBrand(Guid id) => await _constructors
        .Include(c => c.Brand)
        .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<List<Constructor>> GetByBrandId(Guid brandId) => await _constructors
        .Where(c => c.BrandId == brandId)
        .ToListAsync();

    public async Task<Constructor?> GetByName(string name) => await _constructors.FirstOrDefaultAsync(c => c.Name == name);

    public async Task<bool> CheckIfIdExists(Guid id) => await _constructors.AnyAsync(c => c.Id == id);
}