using Context;
using Entities.Models.Standings;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.Standings;

namespace Repositories.Implementations.Standings;

public class BrandsRepository(EfContext context): IBrandsRepository
{
    private readonly DbSet<Brand> _brands = context.Brands;
    
    public async Task<Brand?> GetBrandById(Guid id) => await _brands.FirstOrDefaultAsync(b => b.Id == id);

    public async Task<List<Brand>> GetAllBrands() => await _brands.ToListAsync();
    
    public async Task Create(Brand brand)
    {
        _brands.Add(brand);
        await context.SaveChangesAsync();
    }

    public async Task Update(Brand brand)
    {
        _brands.Update(brand);
        await context.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        var brand = await GetBrandById(id);
        if(brand == null) return;
        
        _brands.Remove(brand);
        await context.SaveChangesAsync();
    }

    public async Task<Brand?> GetByName(string name) => await _brands.
        FirstOrDefaultAsync(b => b.Name.Contains(name, StringComparison.CurrentCultureIgnoreCase));

    public async Task<bool> CheckIfIdExists(Guid id) => await _brands
        .AnyAsync(c => c.Id == id);
}