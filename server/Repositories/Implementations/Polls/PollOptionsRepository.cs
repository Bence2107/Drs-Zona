using Context;
using Entities.Models.Polls;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.Polls;

namespace Repositories.Implementations.Polls;

public class PollOptionsRepository(EfContext context) : IPollOptionsRepository
{
    private readonly DbSet<PollOption> _pollOptions = context.PollOptions;
    
    public async Task<PollOption?> GetPollOptionById(Guid id) => await _pollOptions
        .FirstOrDefaultAsync(x => x.Id == id);
    
    public async Task<List<PollOption>> GetAllPollOptions() => await _pollOptions.ToListAsync();
    
    public async Task Create(PollOption pollOption)
    {
        await _pollOptions.AddAsync(pollOption);
        await context.SaveChangesAsync();
    }

    public async Task Update(PollOption pollOption)
    {
        _pollOptions.Update(pollOption);
        await context.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        var pollOption =  await GetPollOptionById(id);
        if(pollOption == null) return;
        
        _pollOptions.Remove(pollOption);
        await context.SaveChangesAsync();
    }

    public async Task<PollOption?> GetByIdWithPoll(Guid id) => await  _pollOptions
        .Include(po => po.Poll)
        .FirstOrDefaultAsync(po => po.Id == id);

    public async Task<List<PollOption>> GetByPollId(Guid pollId) => await _pollOptions
        .Where(poll => poll.PollId == pollId)
        .ToListAsync();

    public async Task<bool> CheckIfIdExists(Guid id)
    {
        return  await _pollOptions.AnyAsync(x => x.Id == id);
    }
}