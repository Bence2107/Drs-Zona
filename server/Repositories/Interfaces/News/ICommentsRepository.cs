using Entities.Models.News;

namespace Repositories.Interfaces.News;

public interface ICommentsRepository
{
    Comment? GetCommentById(Guid id);
    List<Comment> GetAllComments(Guid id);
    void Add(Comment comment);
    void Update(Comment comment);
    void Delete(Guid id);
    Comment? GetByIdWithUser(Guid id);
    Comment? GetByIdWithArticle(Guid id);
    Comment? GetByIdWithAll(Guid id);
    List<Comment> GetByArticleId(Guid articleId);
    List<Comment> GetUsersComments(Guid userId);
    List<Comment> GetCommentsWithoutReplies(Guid articleId);
    List<Comment> GetRepliesToAComment(Guid replyCommentId);
    bool CheckIfIdExists(Guid id);
}