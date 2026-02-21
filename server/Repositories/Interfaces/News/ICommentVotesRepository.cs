using Entities.Models.News;

namespace Repositories.Interfaces.News;

public interface ICommentVotesRepository
{
    CommentVote? GetVote(Guid userId, Guid commentId);
    void Add(CommentVote vote);
    void Update(CommentVote vote);
    void Delete(CommentVote vote);
}