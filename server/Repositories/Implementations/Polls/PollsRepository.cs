using Context;
using Entities.Models.Polls;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.Polls;

namespace Repositories.Implementations.Polls;

public class PollsRepository(EfContext context) : IPollsRepository
{
    private readonly DbSet<Poll> _polls = context.Polls;
    
    public Poll? GetPollById(int id)
    {
        return _polls.FirstOrDefault(p => p.Id == id);
    }

    public List<Poll> GetAll()
    {
        return _polls.ToList();
    }

    public void Add(Poll poll)
    {
        _polls.Add(poll);
        context.SaveChanges();
    }

    public void Modify(Poll poll)
    {
        _polls.Update(poll);
        context.SaveChanges();
    }

    public void Delete(int id)
    {
        var poll = GetPollById(id);
        if(poll == null) return;
        
        _polls.Remove(poll);
        context.SaveChanges();
    }

    public Poll? GetByIdWithAuthor(int id) => _polls
        .Include(p => p.Author)
        .FirstOrDefault(p => p.Id == id);

    public List<Poll> GetActive() => _polls
        .Where(p => p.IsActive)
        .ToList();
    

    public List<Poll> GetByCreatorId(int authorId) => _polls
        .Where(poll => poll.AuthorId == authorId)
        .ToList();
    

    public List<Poll> GetExpired() => _polls
        .Where(poll => poll.ExpiresAt < DateTime.Now)
        .ToList();
    
    public bool CheckIfIdExists(int id) => _polls.Any(p => p.Id == id);
}