using Entities.Models.Polls;

namespace Repositories.Interfaces.Polls;

public interface IPollOptionsRepository 
{
    Task<PollOption?> GetPollOptionById(Guid id);
    Task<List<PollOption>> GetByPollId(Guid pollId);
    Task Create(PollOption pollOption);
}