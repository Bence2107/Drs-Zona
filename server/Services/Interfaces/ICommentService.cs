using DTOs.News;

namespace Services.Interfaces;

public interface ICommentService
{
    ResponseResult<List<CommentDetailDto>> GetArticleCommentsWithoutReplies(Guid articleId);
    
    ResponseResult<List<CommentDetailDto>> GetCommentReplies(Guid commentId);
    
    ResponseResult<List<CommentDetailDto>> GetUsersComments(Guid userId);
    
    ResponseResult<bool> AddComment(CommentCreateDto commentCreateDto, Guid userId);
    
    ResponseResult<bool> UpdateCommentsContent(CommentContentUpdateDto commentUpdateContentDto);
    
    ResponseResult<bool> UpdateCommentsVote(CommentUpdateVoteDto commentUpdateVoteDto);
    
    ResponseResult<bool> DeleteComment(Guid commentId);
}