using DTOs.News;

namespace Services.Interfaces;

public interface ICommentService
{
    ResponseResult<List<CommentDetailDto>> GetArticleCommentsWithoutReplies(int articleId);
    
    ResponseResult<List<CommentDetailDto>> GetCommentReplies(int commentId);
    
    ResponseResult<List<CommentDetailDto>> GetUsersComments(int userId);
    
    ResponseResult<bool> AddComment(CommentCreateDto commentCreateDto, int userId);
    
    ResponseResult<bool> UpdateCommentsContent(CommentContentUpdateDto commentUpdateContentDto);
    
    ResponseResult<bool> UpdateCommentsVote(CommentUpdateVoteDto commentUpdateVoteDto);
    
    ResponseResult<bool> DeleteComment(int commentId);
}