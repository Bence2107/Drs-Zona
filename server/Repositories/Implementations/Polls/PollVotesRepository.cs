using Context;
using Entities.Models.Polls;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.Polls;

namespace Repositories.Implementations.Polls;

public class PollVotesRepository(EfContext context) : IPollVotesRepository 
{
    private readonly DbSet<PollVote> _votes = context.PollVotes;
    
    public async Task<List<PollVote>> GetAllVotes() => await _votes.ToListAsync();

    public async Task<List<PollVote>> GetByUserId(Guid userId) => await _votes
        .Where(vote => vote.UserId == userId)
        .ToListAsync();

    public async Task<PollVote?> GetUserVoteForPoll(Guid userId, Guid pollId) => await _votes
        .Include(v => v.PollOption) 
        .FirstOrDefaultAsync(v => v.UserId == userId && v.PollOption!.PollId == pollId);

    public async Task<int> GetVoteCount(Guid pollOptionId) => await _votes
        .CountAsync(v => v.PollOptionId == pollOptionId);
    
    public async Task Create(PollVote pollVote)
    {
        await _votes.AddAsync(pollVote);
        await context.SaveChangesAsync();
    }

    public async Task Delete(Guid userId, Guid pollOptionId)
    {
        var vote = await GetVoteById(userId, pollOptionId);
        if(vote == null) return;
        
        _votes.Remove(vote);
        await context.SaveChangesAsync();
    }
    
    private async Task<PollVote?> GetVoteById(Guid userId, Guid pollOptionId) => await _votes
        .FirstOrDefaultAsync(vote => vote.UserId == userId && vote.PollOptionId == pollOptionId);
}