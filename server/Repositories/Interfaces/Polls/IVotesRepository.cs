using Entities.Models.Polls;

namespace Repositories.Interfaces.Polls;

public interface IVotesRepository
{
    Vote? GetVoteById(int userId, int pollOptionId);
    List<Vote> GetAllVotes();
    void Create(Vote vote);
    void Update(Vote vote);
    void Delete(int userId, int pollOptionId);
    List<Vote> GetByUserId(int userId);
    List<Vote> GetByPollOptionId(int pollOptionId);
    int GetVoteCount(int pollOptionId);
    bool CheckIfExists(int userId, int pollOptionId);
}