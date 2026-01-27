using Entities.Models.Polls;

namespace Repositories.Interfaces.Polls;

public interface IPollsRepository
{
    Poll? GetPollById(Guid id);
    List<Poll> GetAll();
    void Add(Poll poll);
    void Modify(Poll poll);
    void Delete(Guid id);
    Poll? GetByIdWithAuthor(Guid id);
    List<Poll> GetActive();
    List<Poll> GetByCreatorId(Guid authorId);
    List<Poll> GetExpired();
    bool CheckIfIdExists(Guid id);
}