using DTOs.News;
using Entities.Models.News;
using Repositories.Interfaces;
using Repositories.Interfaces.News;
using Services.Interfaces;
using Services.Interfaces.images;
using Services.Types;

namespace Services.Implementations;

public class CommentService(
    IArticlesRepository articlesRepo,
    ICommentsRepository commentsRepo, 
    IAuthRepository usersRepo, 
    IUserImageService userImageService,
    ICommentVotesRepository commentVotesRepo
) : ICommentService 
{
    public async Task<ResponseResult<List<CommentDetailDto>>> GetArticleCommentsWithoutReplies(Guid articleId, Guid? currentUserId = null)
    {
        var commentsData = await commentsRepo.GetCommentsWithoutReplies(articleId);
        var comments = new List<CommentDetailDto>();

        foreach (var c in commentsData)
        {
            var vote = await commentVotesRepo.GetVoteForACommment(currentUserId, c.Id);
            var voteStatus = vote == null ? 0 : (vote.IsUpvote ? 1 : -1);
            var replyCount = await commentsRepo.GetNumberOfReplies(c.Id);
            var avatarUrl = await userImageService.GetAvatarUrl(c.UserId);

            comments.Add(new CommentDetailDto(
                Id: c.Id,
                UserId: c.UserId,
                ArticleId: c.ArticleId,
                ArticleSlug: c.Article?.Slug ?? "",
                ReplyToCommentId: c.ReplyToCommentId,
                Username: c.User!.Username,
                UserAvatarUrl: avatarUrl,
                Content: c.Content,
                UpVotes: c.UpVotes,
                DownVotes: c.DownVotes,
                ReplyCount: replyCount,
                DateCreated: c.DateCreated,
                DateUpdated: c.DateUpdated,
                CurrentUserVote: voteStatus 
            ));
        }
    
        return ResponseResult<List<CommentDetailDto>>.Success(comments);
    }

    public async Task<ResponseResult<List<CommentDetailDto>>> GetCommentReplies(Guid commentId, Guid? currentUserId = null)
    {
        var commentsData = await commentsRepo.GetRepliesToAComment(commentId);
        var comments = new List<CommentDetailDto>();

        foreach (var c in commentsData)
        {
            var vote = await commentVotesRepo.GetVoteForACommment(currentUserId, c.Id);
            var voteStatus = vote == null ? 0 : (vote.IsUpvote ? 1 : -1);
            var replyCount = await commentsRepo.GetNumberOfReplies(c.Id);
            var avatarUrl = await userImageService.GetAvatarUrl(c.UserId);
            
            comments.Add(new CommentDetailDto(
                Id: c.Id,
                UserId: c.UserId,
                ArticleId: c.ArticleId,
                ArticleSlug: c.Article?.Slug ?? "",
                ReplyToCommentId: c.ReplyToCommentId,
                Username: c.User!.Username,
                UserAvatarUrl: avatarUrl,
                Content: c.Content,
                UpVotes: c.UpVotes,
                DownVotes: c.DownVotes,
                ReplyCount: replyCount,
                DateCreated: c.DateCreated,
                DateUpdated: c.DateUpdated,
                CurrentUserVote: voteStatus
            ));
        }

        return ResponseResult<List<CommentDetailDto>>.Success(comments);
    }

    public async Task<ResponseResult<List<CommentDetailDto>>> GetUsersComments(Guid userId)
    {
        var commentsData = await commentsRepo.GetUsersComments(userId);
        var comments = new List<CommentDetailDto>();

        foreach (var c in commentsData)
        {
            var vote = await commentVotesRepo.GetVoteForACommment(userId, c.Id);
            var voteStatus = vote == null ? 0 : (vote.IsUpvote ? 1 : -1);
            var replyCount = await commentsRepo.GetNumberOfReplies(c.Id);
            var avatarUrl = await userImageService.GetAvatarUrl(c.UserId);

            comments.Add(new CommentDetailDto(
                Id: c.Id,
                UserId: c.UserId,
                ArticleId: c.ArticleId,
                ArticleSlug: c.Article?.Slug ?? "",
                ReplyToCommentId: c.ReplyToCommentId,
                Username: c.User!.Username,
                UserAvatarUrl: avatarUrl,
                Content: c.Content,
                UpVotes: c.UpVotes,
                DownVotes: c.DownVotes,
                ReplyCount: replyCount,
                DateCreated: c.DateCreated,
                DateUpdated: c.DateUpdated,
                CurrentUserVote: voteStatus
            ));
        }

        return ResponseResult<List<CommentDetailDto>>.Success(comments);
    }

    public async Task<ResponseResult<bool>> Create(CommentCreateDto commentCreateDto, Guid userId)
    {
        if (!await usersRepo.CheckIfIdExists(userId)) return ResponseResult<bool>.Failure("User not found");
        
        if (!await articlesRepo.CheckIfIdExists(commentCreateDto.ArticleId)) return ResponseResult<bool>.Failure("Article not found");
        
        if (commentCreateDto.ReplyToCommentId.HasValue)
        {
            var replyTo = await commentsRepo.GetCommentById(commentCreateDto.ReplyToCommentId.Value);
            if (replyTo is null) return ResponseResult<bool>.Failure("Comment not found");
        }
        
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
        
        await commentsRepo.Create(comment);
        return ResponseResult<bool>.Success(true);
    }

    public async Task<ResponseResult<bool>> UpdateCommentsContent(CommentContentUpdateDto dto)
    {
        var existingComment = await commentsRepo.GetCommentById(dto.Id);
        if (existingComment == null) return ResponseResult<bool>.Failure("A komment nem található");

        if (existingComment.ArticleId != dto.ArticleId) 
            return ResponseResult<bool>.Failure("Nem megfelelő hír");

        existingComment.Content = dto.Content;
        existingComment.DateUpdated = DateTime.UtcNow;

        await commentsRepo.Update(existingComment);
    
        return ResponseResult<bool>.Success(true);
    }

    public async Task<ResponseResult<bool>> UpdateCommentsVote(CommentUpdateVoteDto commentUpdateVoteDto)
    {
        var userId = commentUpdateVoteDto.UserId;
        var commentId = commentUpdateVoteDto.CommentId;
        var isUpvote = commentUpdateVoteDto.IsUpvote;
        
        var comment = await commentsRepo.GetCommentById(commentId);
        if (comment == null) return ResponseResult<bool>.Failure("A komment nem található.");

        var existingVote = await commentVotesRepo.GetVoteForACommment(userId, commentId);

        if (existingVote == null)
        {
            await commentVotesRepo.Create(new CommentVote { UserId = userId, CommentId = commentId, IsUpvote = isUpvote });
            if (isUpvote) comment.UpVotes++; else comment.DownVotes++;
        }
        else if (existingVote.IsUpvote == isUpvote)
        {
            await commentVotesRepo.Delete(existingVote);
            if (isUpvote) comment.UpVotes--; else comment.DownVotes--;
        }
        else
        {
            existingVote.IsUpvote = isUpvote;
            await commentVotesRepo.Update(existingVote);

            if (isUpvote) {
                comment.UpVotes++;
                comment.DownVotes--;
            } else {
                comment.UpVotes--;
                comment.DownVotes++;
            }
        }

        await commentsRepo.Update(comment); 
        return ResponseResult<bool>.Success(true);
    }

    public async Task<ResponseResult<bool>> DeleteComment(Guid commentId)
    {
        var comment = await commentsRepo.GetCommentById(commentId);
        if (comment == null) return ResponseResult<bool>.Failure("Comment not found");

        var replies = await commentsRepo.GetRepliesToAComment(commentId);
        if (replies.Count > 0)
        {
            foreach (var reply in replies)
            {
                await commentsRepo.Delete(reply.Id);
            }
        }
        
        await commentsRepo.Delete(commentId);
        return ResponseResult<bool>.Success(true);
    }
}