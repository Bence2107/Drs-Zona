using Entities.Models.Polls;

namespace Repositories.Interfaces.Polls;

public interface IPollsRepository
{
    Task<Poll?> GetPollById(Guid id);
    Task<List<Poll>> GetAll();
    Task Add(Poll poll);
    Task Modify(Poll poll);
    Task Delete(Guid id);
    Task<Poll?> GetByIdWithAuthor(Guid id);
    Task<List<Poll>> GetActive();
    Task<List<Poll>> GetByCreatorId(Guid authorId);
    Task<List<Poll>> GetExpired();
    Task<bool> CheckIfIdExists(Guid id);
}