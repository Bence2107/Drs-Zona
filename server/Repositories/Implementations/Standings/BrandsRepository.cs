using Context;
using Entities.Models.Standings;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.Standings;

namespace Repositories.Implementations.Standings;

public class BrandsRepository(EfContext context): IBrandsRepository
{
    private readonly DbSet<Brand> _brands = context.Brands;
    
    public Brand? GetBrandById(int id) => _brands.FirstOrDefault(b => b.Id == id);
    

    public List<Brand> GetAllBrands() => _brands.ToList();
    
    public void Create(Brand brand)
    {
        _brands.Add(brand);
        context.SaveChanges();
    }

    public void Update(Brand brand)
    {
        _brands.Update(brand);
        context.SaveChanges();
    }

    public void Delete(int id)
    {
        var brand = GetBrandById(id);
        if(brand == null) return;
        
        _brands.Remove(brand);
        context.SaveChanges();
    }

    public Brand? GetByName(string name) => _brands.FirstOrDefault(b => b.Name == name);

    public bool CheckIfIdExists(int id) => _brands.Any(c => c.Id == id);
}