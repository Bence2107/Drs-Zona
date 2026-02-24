using Entities.Models.Polls;

namespace Repositories.Interfaces.Polls;

public interface IPollVotesRepository
{
    Task<PollVote?> GetVoteById(Guid userId, Guid pollOptionId);
    Task<List<PollVote>> GetAllVotes();
    Task Create(PollVote pollVote);
    Task Update(PollVote pollVote);
    Task Delete(Guid userId, Guid pollOptionId);
    Task<List<PollVote>> GetByUserId(Guid userId);
    Task<List<PollVote>> GetByPollOptionId(Guid pollOptionId);
    Task<PollVote?> GetUserVoteForPoll(Guid userId, Guid pollId);
    Task<int> GetVoteCount(Guid pollOptionId);
    Task<bool> CheckIfExists(Guid userId, Guid pollOptionId);
}