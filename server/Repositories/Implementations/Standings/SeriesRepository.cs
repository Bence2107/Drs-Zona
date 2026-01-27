using Context;
using Entities.Models.Standings;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.Standings;

namespace Repositories.Implementations.Standings;

public class SeriesRepository(EfContext context) : ISeriesRepository
{
    private readonly DbSet<Series> _series = context.Series;
    
    public Series? GetSeriesById(Guid id) => _series.FirstOrDefault(d => d.Id == id);

    public List<Series> GetAllSeries() => _series.ToList();

    public void Create(Series series)
    {
       _series.Add(series);
       context.SaveChanges();
    }

    public void Update(Series series)
    {
        _series.Add(series);
        context.SaveChanges();
    }

    public void Delete(Guid id)
    {
        var series = GetSeriesById(id);
        if (series == null) return;
        
        _series.Remove(series);
        context.SaveChanges();
    }

    public Series? GetByName(string name) => _series.FirstOrDefault(d => d.Name == name);

    public bool CheckIfIdExists(Guid id) => _series.Any(d => d.Id == id);
}