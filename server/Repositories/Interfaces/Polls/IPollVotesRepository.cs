using Entities.Models.Polls;

namespace Repositories.Interfaces.Polls;

public interface IPollVotesRepository 
{
    Task<List<PollVote>> GetByUserId(Guid userId);
    Task<PollVote?> GetUserVoteForPoll(Guid userId, Guid pollId);
    Task<int> GetVoteCount(Guid pollOptionId);
    Task Create(PollVote pollVote);
    Task Delete(Guid userId, Guid pollOptionId);
}