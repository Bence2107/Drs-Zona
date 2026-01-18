using Entities.Models.Polls;

namespace Repositories.Interfaces.Polls;

public interface IPollOptionsRepository
{
    PollOption? GetPollOptionById(int id);
    List<PollOption> GetAllPollOptions();
    void Create(PollOption pollOption);
    void Update(PollOption pollOption);
    void Delete(int id);
    PollOption? GetByIdWithPoll(int id);
    List<PollOption> GetByPollId(int pollId);
    bool CheckIfIdExists(int id);
}