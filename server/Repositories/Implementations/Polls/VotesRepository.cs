using Context;
using Entities.Models.Polls;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.Polls;

namespace Repositories.Implementations.Polls;

public class VotesRepository(EfContext context) : IVotesRepository
{
    private readonly DbSet<PollVote> _votes = context.PollVotes;
    public PollVote? GetVoteById(Guid userId, Guid pollOptionId) =>_votes
        .FirstOrDefault(vote => vote.UserId == userId && vote.PollOptionId == pollOptionId);
    
    public List<PollVote> GetAllVotes() => _votes.ToList();
    
    public void Create(PollVote pollVote)
    {
        _votes.Add(pollVote);
        context.SaveChanges();
    }

    public void Update(PollVote pollVote)
    {
        _votes.Update(pollVote);
        context.SaveChanges();
    }

    public void Delete(Guid userId, Guid pollOptionId)
    {
        var vote = GetVoteById(userId, pollOptionId);
        if(vote == null) return;
        
        _votes.Remove(vote);
        context.SaveChanges();
    }

    public List<PollVote> GetByUserId(Guid userId) =>_votes
        .Where(vote => vote.UserId == userId)
        .ToList();
    

    public List<PollVote> GetByPollOptionId(Guid pollOptionId) => _votes
        .Where(vote => vote.PollOptionId == pollOptionId)
        .ToList();

    public PollVote? GetUserVoteForPoll(Guid userId, Guid pollId) => _votes
        .Include(v => v.PollOption) 
        .FirstOrDefault(v => v.UserId == userId && v.PollOption!.PollId == pollId);

    public int GetVoteCount(Guid pollOptionId) => _votes.Count(v => v.PollOptionId == pollOptionId);
    
    public bool CheckIfExists(Guid userId, Guid pollOptionId) => _votes.Any(v => v.UserId == userId && v.PollOptionId == pollOptionId);
}