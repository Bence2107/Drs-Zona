using Entities.Models.News;

namespace Repositories.Interfaces.News;

public interface ICommentVotesRepository 
{
    Task<CommentVote?> GetVoteForACommment(Guid? userId, Guid commentId);
    Task Create(CommentVote vote);
    Task Update(CommentVote vote);
    Task Delete(CommentVote vote);
}