using Entities.Models.Polls;

namespace Repositories.Interfaces.Polls;

public interface IPollOptionsRepository
{
    Task<PollOption?> GetPollOptionById(Guid id);
    Task<List<PollOption>> GetAllPollOptions();
    Task Create(PollOption pollOption);
    Task Update(PollOption pollOption);
    Task Delete(Guid id);
    Task<PollOption?> GetByIdWithPoll(Guid id);
    Task<List<PollOption>> GetByPollId(Guid pollId);
    Task<bool> CheckIfIdExists(Guid id);
}