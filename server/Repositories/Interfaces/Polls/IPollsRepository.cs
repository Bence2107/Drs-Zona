using Entities.Models.Polls;

namespace Repositories.Interfaces.Polls;

public interface IPollsRepository
{
    Poll? GetPollById(int id);
    List<Poll> GetAll();
    void Add(Poll poll);
    void Modify(Poll poll);
    void Delete(int id);
    Poll? GetByIdWithAuthor(int id);
    List<Poll> GetActive();
    List<Poll> GetByCreatorId(int authorId);
    List<Poll> GetExpired();
    bool CheckIfIdExists(int id);
}