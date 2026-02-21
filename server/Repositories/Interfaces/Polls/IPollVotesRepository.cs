using Entities.Models.Polls;

namespace Repositories.Interfaces.Polls;

public interface IPollVotesRepository
{
    PollVote? GetVoteById(Guid userId, Guid pollOptionId);
    List<PollVote> GetAllVotes();
    void Create(PollVote pollVote);
    void Update(PollVote pollVote);
    void Delete(Guid userId, Guid pollOptionId);
    List<PollVote> GetByUserId(Guid userId);
    List<PollVote> GetByPollOptionId(Guid pollOptionId);
    PollVote? GetUserVoteForPoll(Guid userId, Guid pollId);
    int GetVoteCount(Guid pollOptionId);
    bool CheckIfExists(Guid userId, Guid pollOptionId);
}