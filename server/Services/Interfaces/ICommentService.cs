using DTOs.News;

namespace Services.Interfaces;

public interface ICommentService
{
    ResponseResult<List<CommentDetailDto>> GetArticleCommentsWithoutReplies(Guid articleId, Guid? currentUserId = null);
    
    ResponseResult<List<CommentDetailDto>> GetCommentReplies(Guid commentId, Guid? currentUserId = null);
    
    ResponseResult<List<CommentDetailDto>> GetUsersComments(Guid userId, Guid? currentUserId = null);
    
    ResponseResult<bool> AddComment(CommentCreateDto commentCreateDto, Guid id);
    
    ResponseResult<bool> UpdateCommentsContent(CommentContentUpdateDto commentUpdateContentDto);

    ResponseResult<bool> UpdateCommentsVote(CommentUpdateVoteDto commentUpdateVoteDto);
    
    ResponseResult<bool> DeleteComment(Guid commentId);
}