using Context;
using Entities.Models.Standings;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.Standings;

namespace Repositories.Implementations.Standings;

public class ConstructorsRepository(EfContext context) : IConstructorsRepository
{
    private readonly DbSet<Constructor> _constructors = context.Constructors;
    
    public Constructor? GetConstructorById(Guid id) => _constructors.FirstOrDefault(c => c.Id == id);

    public List<Constructor> GetAllConstructor() => _constructors.ToList();

    public void Create(Constructor constructor)
    {
        _constructors.Add(constructor);
        context.SaveChanges();
    }

    public void Update(Constructor constructor)
    {
        _constructors.Update(constructor);
        context.SaveChanges();
    }

    public void Delete(Guid id)
    {
        var  constructor = GetConstructorById(id);
        if(constructor == null) return;
        
        _constructors.Remove(constructor);
        context.SaveChanges();
    }

    public Constructor? GetByIdWithBrand(Guid id) => _constructors
        .Include(c => c.Brand)
        .FirstOrDefault(c => c.Id == id);

    public List<Constructor> GetByBrandId(Guid brandId) => _constructors
        .Where(c => c.BrandId == brandId)
        .ToList();

    public Constructor? GetByName(string name) => _constructors.FirstOrDefault(c => c.Name == name);

    public bool CheckIfIdExists(Guid id) => _constructors.Any(c => c.Id == id);
}