using Entities.Models.News;

namespace Repositories.Interfaces.News;

public interface ICommentVotesRepository
{
    CommentVote? GetVote(Guid userId, Guid commentId);
    List<CommentVote> GetVotesByUser(Guid userId); 
    void Add(CommentVote vote);
    void Update(CommentVote vote);
    void Delete(CommentVote vote);
}