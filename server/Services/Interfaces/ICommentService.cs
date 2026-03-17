using DTOs.News;
using Services.Types;

namespace Services.Interfaces;

public interface ICommentService
{
    Task<ResponseResult<List<CommentDetailDto>>> GetArticleCommentsWithoutReplies(Guid articleId, Guid? currentUserId = null);
    Task<ResponseResult<List<CommentDetailDto>>> GetCommentReplies(Guid commentId, Guid? currentUserId = null);
    Task<ResponseResult<List<CommentDetailDto>>> GetUsersComments(Guid userId);
    Task<ResponseResult<bool>> AddComment(CommentCreateDto commentCreateDto, Guid id);
    Task<ResponseResult<bool>> UpdateCommentsContent(CommentContentUpdateDto commentUpdateContentDto);
    Task<ResponseResult<bool>> UpdateCommentsVote(CommentUpdateVoteDto commentUpdateVoteDto);
    Task<ResponseResult<bool>> DeleteComment(Guid commentId);
}