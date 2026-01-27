using Entities.Models.Polls;

namespace Repositories.Interfaces.Polls;

public interface IPollOptionsRepository
{
    PollOption? GetPollOptionById(Guid id);
    List<PollOption> GetAllPollOptions();
    void Create(PollOption pollOption);
    void Update(PollOption pollOption);
    void Delete(Guid id);
    PollOption? GetByIdWithPoll(Guid id);
    List<PollOption> GetByPollId(Guid pollId);
    bool CheckIfIdExists(Guid id);
}