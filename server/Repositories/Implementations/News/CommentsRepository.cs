using Context;
using Entities.Models.News;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.News;

namespace Repositories.Implementations.News;

public class CommentsRepository(EfContext context) : ICommentsRepository
{
    private readonly DbSet<Comment> _comments = context.Comments;
    
    public Comment? GetCommentById(Guid id) => _comments.FirstOrDefault(comment => comment.Id == id);
    
    public List<Comment> GetAllComments(Guid id) => _comments.ToList();
    
    public void Add(Comment comment)
    {
        _comments.Add(comment);
        context.SaveChanges();
    }

    public void Update(Comment comment)
    {
        _comments.Update(comment);
        context.SaveChanges();
    }

    public void Delete(Guid id)
    {
        var comment = GetCommentById(id);
        if(comment == null) return;
        _comments.Remove(comment);
        context.SaveChanges();
    }

    public Comment? GetByIdWithUser(Guid id) => _comments
        .Include(comment => comment.User)
        .FirstOrDefault(comment => comment.Id == id);
    

    public Comment? GetByIdWithArticle(Guid id) => _comments
        .Include(comment => comment.Article)
        .FirstOrDefault(comment => comment.Id == id);

    public Comment? GetByIdWithAll(Guid id) => _comments
        .Include(comment => comment.User)
        .Include(comment => comment.Article)
        .FirstOrDefault(comment => comment.Id == id);

    public List<Comment> GetByArticleId(Guid articleId) => _comments
        .Where(comment => comment.ArticleId == articleId)
        .ToList();
    
    public List<Comment> GetUsersComments(Guid userId) => _comments
        .Where(comment => comment.UserId == userId)
        .ToList();

    public List<Comment> GetCommentsWithoutReplies(Guid articleId) => _comments
        .Include(comment => comment.User)
        .Where(comment => comment.ArticleId == articleId && comment.ReplyToCommentId == null)
        .ToList();

    public List<Comment> GetRepliesToAComment(Guid replyCommentId) => _comments
        .Include(comment => comment.User)
        .Where(comment => comment.ReplyToCommentId == replyCommentId)
        .ToList();
    public bool CheckIfIdExists(Guid id) => _comments.Any(comment => comment.Id == id);
}