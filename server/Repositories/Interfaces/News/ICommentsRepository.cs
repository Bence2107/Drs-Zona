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
    List<Comment> GetUsersComments(int userId);
    List<Comment> GetCommentsWithoutReplies(int articleId);
    List<Comment> GetRepliesToAComment(int replyCommentId);
    bool CheckIfIdExists(int id);
}