using Context;
using Entities.Models.Standings;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.Standings;

namespace Repositories.Implementations.Standings;

public class ConstructorCompetitionRepository(EfContext context) : IConstructorCompetitionRepository
{
    private readonly DbSet<ConstructorCompetition> _constructorCompetitions = context.ConstructorCompetitions;

    public ConstructorCompetition? GetConstructorCompetitionById(int constructorId, int championshipId) => _constructorCompetitions
        .FirstOrDefault(cc => cc.ConstructorId == constructorId && cc.ConstructorId == championshipId);

    public List<ConstructorCompetition> GetAllConstructorCompetitions() => _constructorCompetitions.ToList();

    public void Create(ConstructorCompetition constructorCompetition)
    {
        _constructorCompetitions.Add(constructorCompetition);
        context.SaveChanges();
    }

    public void Update(ConstructorCompetition constructorCompetition)
    {
        _constructorCompetitions.Update(constructorCompetition);
        context.SaveChanges();
    }

    public void Delete(int constructorId, int championshipId)
    {
        var constCompetition = GetConstructorCompetitionById(constructorId, championshipId);
        if(constCompetition == null) return;
        
        _constructorCompetitions.Remove(constCompetition);
        context.SaveChanges();
    }

    public List<ConstructorCompetition> GetByConstructorId(int constructorId) => _constructorCompetitions
        .Where(cc => cc.ConstructorId == constructorId)
        .ToList();

    public List<ConstructorCompetition> GetByChampionshipId(int champId) => _constructorCompetitions
        .Where(cc => cc.ConstChampId == champId)
        .ToList();

    public bool CheckIfExists(int constructorId, int champId) => _constructorCompetitions
        .Any(cc => cc.ConstructorId == constructorId && cc.ConstChampId == champId);
}