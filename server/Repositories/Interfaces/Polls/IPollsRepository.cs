using Entities.Models.Polls;

namespace Repositories.Interfaces.Polls;

public interface IPollsRepository
{
    Task<Poll?> GetPollById(Guid id);
    Task<List<Poll>> GetAll(string? tag = null);
    Task Add(Poll poll);
    Task Delete(Guid id);
    Task<Poll?> GetByIdWithAuthor(Guid id);
    Task<List<Poll>> GetActive(string? tag = null);
    Task<List<Poll>> GetByCreatorId(Guid authorId, string? tag = null);
    Task<List<Poll>> GetExpired(string? tag = null);
    Task<bool> CheckIfIdExists(Guid id);
}