using Context;
using Entities.Models.Polls;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.Polls;

namespace Repositories.Implementations.Polls;

public class VotesRepository(EfContext context) : IVotesRepository
{
    private readonly DbSet<Vote> _votes = context.Votes;
    public Vote? GetVoteById(Guid userId, Guid pollOptionId) =>_votes
        .FirstOrDefault(vote => vote.UserId == userId && vote.PollOptionId == pollOptionId);
    
    public List<Vote> GetAllVotes() => _votes.ToList();
    
    public void Create(Vote vote)
    {
        _votes.Add(vote);
        context.SaveChanges();
    }

    public void Update(Vote vote)
    {
        _votes.Update(vote);
        context.SaveChanges();
    }

    public void Delete(Guid userId, Guid pollOptionId)
    {
        var vote = GetVoteById(userId, pollOptionId);
        if(vote == null) return;
        
        _votes.Remove(vote);
        context.SaveChanges();
    }

    public List<Vote> GetByUserId(Guid userId) =>_votes
        .Where(vote => vote.UserId == userId)
        .ToList();
    

    public List<Vote> GetByPollOptionId(Guid pollOptionId) => _votes
        .Where(vote => vote.PollOptionId == pollOptionId)
        .ToList();

    public Vote? GetUserVoteForPoll(Guid userId, Guid pollId) => _votes
        .Include(v => v.PollOption) 
        .FirstOrDefault(v => v.UserId == userId && v.PollOption!.PollId == pollId);

    public int GetVoteCount(Guid pollOptionId) => _votes.Count(v => v.PollOptionId == pollOptionId);
    
    public bool CheckIfExists(Guid userId, Guid pollOptionId) => _votes.Any(v => v.UserId == userId && v.PollOptionId == pollOptionId);
}