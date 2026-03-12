using Context;
using Entities.Models.Polls;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.Polls;

namespace Repositories.Implementations.Polls;

public class PollsRepository(EfContext context) : IPollsRepository
{
    private readonly DbSet<Poll> _polls = context.Polls;
    
    public Task<Poll?> GetPollById(Guid id) => _polls.FirstOrDefaultAsync(p => p.Id == id);

    public Task<List<Poll>> GetAll(string? tag = null) => 
        _polls
            .Where(p => tag == null || p.Tag == tag)
            .ToListAsync();

    public async Task<List<Poll>> GetActive(string? tag = null) => 
        await _polls
            .Where(p => p.IsActive && (tag == null || p.Tag == tag))
            .ToListAsync();

    public async Task<List<Poll>> GetExpired(string? tag = null) => 
        await _polls
            .Where(p => p.ExpiresAt < DateTime.Now && (tag == null || p.Tag == tag))
            .OrderBy(p => p.ExpiresAt)
            .ToListAsync();

    public async Task<List<Poll>> GetByCreatorId(Guid authorId, string? tag = null) => 
        await _polls
            .Where(p => p.AuthorId == authorId && (tag == null || p.Tag == tag))
            .ToListAsync();
    
    public async Task Add(Poll poll)
    {
        _polls.Add(poll);
        await context.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        var poll = await GetPollById(id);
        if(poll == null) return;
        
        _polls.Remove(poll);
        await context.SaveChangesAsync();
    }

    public async Task<Poll?> GetByIdWithAuthor(Guid id) => await _polls
        .Include(p => p.Author)
        .FirstOrDefaultAsync(p => p.Id == id);
    
    
    public async Task<bool> CheckIfIdExists(Guid id) => await _polls.AnyAsync(p => p.Id == id);
}