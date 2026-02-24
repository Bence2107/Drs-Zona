using Entities.Models.News;

namespace Repositories.Interfaces.News;

public interface ICommentsRepository
{
    Task<Comment?> GetCommentById(Guid id);
    Task Add(Comment comment);
    Task Update(Comment comment);
    Task Delete(Guid id);
    Task<List<Comment>> GetUsersComments(Guid userId);
    Task<List<Comment>> GetCommentsWithoutReplies(Guid articleId);
    Task<List<Comment>> GetRepliesToAComment(Guid replyCommentId);
    Task<int> GetNumberOfReplies(Guid commentId);
}