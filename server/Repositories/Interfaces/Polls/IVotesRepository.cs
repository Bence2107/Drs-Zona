using Entities.Models.Polls;

namespace Repositories.Interfaces.Polls;

public interface IVotesRepository
{
    Vote? GetVoteById(Guid userId, Guid pollOptionId);
    List<Vote> GetAllVotes();
    void Create(Vote vote);
    void Update(Vote vote);
    void Delete(Guid userId, Guid pollOptionId);
    List<Vote> GetByUserId(Guid userId);
    List<Vote> GetByPollOptionId(Guid pollOptionId);
    Vote? GetUserVoteForPoll(Guid userId, Guid pollId);
    int GetVoteCount(Guid pollOptionId);
    bool CheckIfExists(Guid userId, Guid pollOptionId);
}