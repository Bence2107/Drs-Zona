using DTOs.News;
using Entities.Models.News;
using Repositories.Interfaces;
using Repositories.Interfaces.News;
using Services.Interfaces;
using Services.Interfaces.images;

namespace Services.Implementations;

public class CommentService(
    ICommentsRepository commentsRepo, 
    IAuthRepository usersRepo, 
    IUserImageService userImageService) 
: ICommentService
{
    public ResponseResult<List<CommentDetailDto>> GetArticleCommentsWithoutReplies(Guid articleId)
    {
        var comments = commentsRepo.GetCommentsWithoutReplies(articleId).Select(c => new CommentDetailDto(
            Id: c.Id,
            UserId: c.UserId,
            ArticleId: c.ArticleId,
            ReplyToCommentId: c.ReplyToCommentId,
            Username: c.User!.Username,
            UserAvatarUrl: userImageService.GetAvatarUrl(c.UserId),
            Content: c.Content,
            UpVotes: c.UpVotes,
            DownVotes: c.DownVotes,
            ReplyCount: commentsRepo.GetNumberOfReplies(c.Id),
            DateCreated: c.DateCreated,
            DateUpdated: c.DateUpdated
        )).ToList();
        
        return ResponseResult<List<CommentDetailDto>>.Success(comments);
    }

    public ResponseResult<List<CommentDetailDto>> GetCommentReplies(Guid commentId)
    {
        var repliesToComment = commentsRepo.GetRepliesToAComment(commentId).Select(c=> new CommentDetailDto(
            Id: c.Id,
            UserId: c.UserId,
            ArticleId: c.ArticleId,
            ReplyToCommentId: c.ReplyToCommentId,
            Username: c.User!.Username,
            UserAvatarUrl: userImageService.GetAvatarUrl(c.UserId),
            Content: c.Content,
            UpVotes: c.UpVotes,
            DownVotes: c.DownVotes,
            ReplyCount: commentsRepo.GetNumberOfReplies(c.Id),
            DateCreated: c.DateCreated,
            DateUpdated: c.DateUpdated
            )).ToList();
        return ResponseResult<List<CommentDetailDto>>.Success(repliesToComment);
    }

    public ResponseResult<List<CommentDetailDto>> GetUsersComments(Guid userId)
    {
        var repliesToComment = commentsRepo.GetUsersComments(userId).Select(c=> new CommentDetailDto(
            Id: c.Id,
            UserId: c.UserId,
            ArticleId: c.ArticleId,
            ReplyToCommentId: c.ReplyToCommentId,
            Username: c.User!.Username,
            UserAvatarUrl: userImageService.GetAvatarUrl(c.UserId),
            Content: c.Content,
            UpVotes: c.UpVotes,
            DownVotes: c.DownVotes,
            ReplyCount: commentsRepo.GetNumberOfReplies(c.Id),
            DateCreated: c.DateCreated,
            DateUpdated: c.DateUpdated
        )).ToList();
        return ResponseResult<List<CommentDetailDto>>.Success(repliesToComment);
    }

    public ResponseResult<bool> AddComment(CommentCreateDto commentCreateDto, Guid userId)
    {
        if (!usersRepo.CheckIfIdExists(userId)) return ResponseResult<bool>.Failure("User not found");
        
        var comment = new Comment
        {
            UserId = userId,
            ArticleId = commentCreateDto.ArticleId,
            ReplyToCommentId = null,
            Content = commentCreateDto.Content,
            UpVotes = 0,
            DownVotes = 0,
            DateCreated = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow
        };
        
        if (commentCreateDto.ReplyToCommentId.HasValue)
        {
            comment.ReplyToCommentId = commentCreateDto.ReplyToCommentId.Value;
        }
        
        commentsRepo.Add(comment);
        return ResponseResult<bool>.Success(true);
    }

    public ResponseResult<bool> UpdateCommentsContent(CommentContentUpdateDto dto)
    {
        var existingComment = commentsRepo.GetCommentById(dto.Id);
        if (existingComment == null) return ResponseResult<bool>.Failure("A komment nem található");

        if (existingComment.ArticleId != dto.ArticleId) 
            return ResponseResult<bool>.Failure("Nem megfelelő hír");

        existingComment.Content = dto.Content;
        existingComment.DateUpdated = DateTime.UtcNow;

        commentsRepo.Update(existingComment);
    
        return ResponseResult<bool>.Success(true);
    }

    public ResponseResult<bool> UpdateCommentsVote(CommentUpdateVoteDto commentUpdateVoteDto)
    {
        var existingComment = commentsRepo.GetCommentById(commentUpdateVoteDto.Id);
        if (existingComment == null) return ResponseResult<bool>.Failure("Comment not found");
        
        var articleToComment = commentsRepo.GetByIdWithArticle(commentUpdateVoteDto.ArticleId);
        if (articleToComment == null) return ResponseResult<bool>.Failure("Article not found");
        
        
        var comment = new Comment
        {
            Id = articleToComment.Id,
            UserId = articleToComment.UserId,
            ArticleId = articleToComment.ArticleId,
            ReplyToCommentId = articleToComment.ReplyToCommentId,
            Content = articleToComment.Content,
            UpVotes = commentUpdateVoteDto.UpVotes,
            DownVotes = commentUpdateVoteDto.DownVotes,
            DateCreated = articleToComment.DateCreated,
            DateUpdated = DateTime.Now
        };
        commentsRepo.Update(comment);
        return ResponseResult<bool>.Success(true);
    }

    public ResponseResult<bool> DeleteComment(Guid commentId)
    {
        var comment = commentsRepo.GetCommentById(commentId);
        if (comment == null) return ResponseResult<bool>.Failure("Comment not found");

        var replies = commentsRepo.GetRepliesToAComment(commentId);
        if (replies.Count > 0)
        {
            foreach (var reply in replies)
            {
                commentsRepo.Delete(reply.Id);
            }
        }
        
        commentsRepo.Delete(commentId);
        return ResponseResult<bool>.Success(true);
    }
}