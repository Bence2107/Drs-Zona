using DTOs.News;
using Entities.Models.News;
using Repositories.Interfaces;
using Repositories.Interfaces.News;
using Services.Interfaces;

namespace Services.Implementations;

public class CommentService(ICommentsRepository commentsRepo, IAuthRepository usersRepo) : ICommentService
{
    public ResponseResult<List<CommentDetailDto>> GetArticleCommentsWithoutReplies(Guid articleId)
    {
        var comments = commentsRepo.GetCommentsWithoutReplies(articleId).Select(c => new CommentDetailDto(
            Id: c.Id,
            UserId: c.UserId,
            ReplyToCommentId: c.ReplyToCommentId,
            Username: c.User!.Username,
            Content: c.Content,
            UpVotes: c.UpVotes,
            DownVotes: c.DownVotes,
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
            ReplyToCommentId: c.ReplyToCommentId,
            Username: c.User!.Username,
            Content: c.Content,
            UpVotes: c.UpVotes,
            DownVotes: c.DownVotes,
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
            ReplyToCommentId: c.ReplyToCommentId,
            Username: c.User!.Username,
            Content: c.Content,
            UpVotes: c.UpVotes,
            DownVotes: c.DownVotes,
            DateCreated: c.DateCreated,
            DateUpdated: c.DateUpdated
        )).ToList();
        return ResponseResult<List<CommentDetailDto>>.Success(repliesToComment);
    }

    public ResponseResult<bool> AddComment(CommentCreateDto commentCreateDto, Guid id)
    {
        var articleToComment = commentsRepo.GetByIdWithArticle(commentCreateDto.ArticleId);
        if (articleToComment == null) return ResponseResult<bool>.Failure("Article not found");
        
        if (!usersRepo.CheckIfIdExists(id)) return ResponseResult<bool>.Failure("User not found");
        
        var comment = new Comment
        {
            UserId = articleToComment.UserId,
            ArticleId = articleToComment.ArticleId,
            ReplyToCommentId = null,
            Content = commentCreateDto.Content,
            UpVotes = 0,
            DownVotes = 0,
            DateCreated = DateTime.Now,
            DateUpdated = DateTime.Now
        };
        
        if (commentCreateDto.ReplyToCommentId.HasValue)
        {
            comment.ReplyToCommentId = commentCreateDto.ReplyToCommentId.Value;
        }
        
        commentsRepo.Add(comment);
        return ResponseResult<bool>.Success(true);
    }

    public ResponseResult<bool> UpdateCommentsContent(CommentContentUpdateDto commentContentUpdateDto)
    {
        var existingComment = commentsRepo.GetCommentById(commentContentUpdateDto.Id);
        if (existingComment == null) return ResponseResult<bool>.Failure("Comment not found");
        
        var articleToComment = commentsRepo.GetByIdWithArticle(commentContentUpdateDto.ArticleId);
        if (articleToComment == null) return ResponseResult<bool>.Failure("Article not found");
        
        
        var comment = new Comment
        {
            Id = articleToComment.Id,
            UserId = articleToComment.UserId,
            ArticleId = articleToComment.ArticleId,
            ReplyToCommentId = articleToComment.ReplyToCommentId,
            Content = commentContentUpdateDto.Content,
            UpVotes = articleToComment.UpVotes,
            DownVotes = articleToComment.DownVotes,
            DateCreated = articleToComment.DateCreated,
            DateUpdated = DateTime.Now
        };
        
        commentsRepo.Update(comment);
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