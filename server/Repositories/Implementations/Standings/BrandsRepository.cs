using Context;
using Entities.Models.Standings;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.Standings;

namespace Repositories.Implementations.Standings;

public class BrandsRepository(EfContext context): IBrandsRepository
{
    private readonly DbSet<Brand> _brands = context.Brands;

    public async Task<List<Brand>> GetAllBrands() => await _brands.ToListAsync();
    
    public async Task<bool> CheckIfIdExists(Guid id) => await _brands
        .AnyAsync(c => c.Id == id);
}