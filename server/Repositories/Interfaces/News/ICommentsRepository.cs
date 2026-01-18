using Entities.Models.News;

namespace Repositories.Interfaces.News;

public interface ICommentsRepository
{
    Comment? GetCommentById(int id);
    List<Comment> GetAllComments(int id);
    void Add(Comment comment);
    void Update(Comment comment);
    void Delete(int id);
    Comment? GetByIdWithUser(int id);
    Comment? GetByIdWithArticle(int id);
    Comment? GetByIdWithAll(int id);
    List<Comment> GetByArticleId(int articleId);
    List<Comment> GetByUserId(int userId);
    List<Comment> GetReplies(int replyCommentId);
    bool CheckIfIdExists(int id);
}