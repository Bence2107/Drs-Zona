using Context;
using Entities.Models.News;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.News;

namespace Repositories.Implementations.News;

public class CommentsRepository(EfContext context) : ICommentsRepository 
{
    private readonly DbSet<Comment> _comments = context.Comments;
    
    public async Task<Comment?> GetCommentById(Guid id) => await _comments.FirstOrDefaultAsync(comment => comment.Id == id);
    
    public async Task<List<Comment>> GetUsersComments(Guid userId)
    {
        var userComments = await _comments
            .Include(c => c.User)
            .Include(c => c.Article)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        var replyIds = userComments
            .Where(c => c.ReplyToCommentId != null)
            .Select(c => c.ReplyToCommentId!.Value)
            .ToHashSet();

        var parentComments = await _comments
            .Where(c => replyIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id);

        var filteredComments = userComments
            .Where(c => c.ReplyToCommentId == null || !HasRootInOwnComments(c, userComments, parentComments))
            .ToList();

        return filteredComments;
    }
    
    public async Task<List<Comment>> GetCommentsWithoutReplies(Guid articleId) => await _comments
        .Include(comment => comment.User)
        .Where(comment => comment.ArticleId == articleId && comment.ReplyToCommentId == null)
        .ToListAsync();

    public async Task<List<Comment>> GetRepliesToAComment(Guid replyCommentId) => await _comments
        .Include(comment => comment.User)
        .Where(comment => comment.ReplyToCommentId == replyCommentId)
        .ToListAsync();

    public async Task<int> GetNumberOfReplies(Guid commentId) => await _comments
        .CountAsync(comment => comment.ReplyToCommentId == commentId);
    
    public async Task Create(Comment comment)
    {
        await _comments.AddAsync(comment);
        await context.SaveChangesAsync();
    }

    public async Task Update(Comment comment)
    {
        _comments.Update(comment);
        await context.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        var comment = await GetCommentById(id);
        if(comment == null) return;
        _comments.Remove(comment);
        await context.SaveChangesAsync();
    } 
    
    private static bool HasRootInOwnComments(Comment comment, List<Comment> userComments, Dictionary<Guid, Comment> parentMap)
    {
        var current = comment;
        while (current.ReplyToCommentId != null)
        {
            if (userComments.Any(c => c.Id == current.ReplyToCommentId))
                return true;

            if (!parentMap.TryGetValue(current.ReplyToCommentId.Value, out var parent))
                break;

            current = parent;
        }
        return false;
    }
}