using DTOs.News;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Drs_Zona.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController(ICommentService commentService): ControllerBase
{
    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var guid) ? guid : null;
    }
    
    [HttpGet("getCommentsWithoutReplies/{articleId:guid}")]
    public async Task<ActionResult<List<CommentDetailDto>>> GetCommentsWithoutReplies([FromRoute] Guid articleId)
    {
        var currentUserId = GetCurrentUserId();
        var response = await commentService.GetArticleCommentsWithoutReplies(articleId, currentUserId);
        if (!response.IsSuccess)
        {
            return BadRequest(new
            {
                response.ErrorField, 
                response.Message
            });
        }
        
        return Ok(response.Value);
    }
    
    [HttpGet("getCommentReplies/{commentId:guid}")]
    public async Task<ActionResult<List<CommentDetailDto>>> GetCommentReplies([FromRoute] Guid commentId)
    {
        var currentUserId = GetCurrentUserId();
        var response = await commentService.GetCommentReplies(commentId, currentUserId);
        if (!response.IsSuccess)
        {
            return BadRequest(new
            {
                response.ErrorField, 
                response.Message
            });
        }
        
        return Ok(response.Value);
    }
    
    [HttpGet("getUsersComments/{userId:guid}")]
    public async Task<ActionResult<List<CommentDetailDto>>> GetUsersComments([FromRoute] Guid userId)
    {
        var response = await commentService.GetUsersComments(userId);
        if (!response.IsSuccess)
        {
            return BadRequest(new
            {
                response.ErrorField, 
                response.Message
            });
        }
        
        return Ok(response.Value);
    }
    
    [HttpPost("create/{userId:guid}")]
    public async Task<ActionResult> Create([FromBody]CommentCreateDto dto, [FromRoute]Guid userId)
    {
        var result = await commentService.AddComment(dto, userId);

        if (!result.IsSuccess)
        {
            return BadRequest(new
            {
                result.ErrorField, 
                result.Message
            });
        }

        return Ok(result.Value);
    }
    
    [HttpPost("updateContent")]
    public async Task<ActionResult> UpdateContent([FromBody]CommentContentUpdateDto commentUpdateVoteDto)
    {
        var result = await commentService.UpdateCommentsContent(commentUpdateVoteDto);
        if (!result.IsSuccess)
        {
            return BadRequest(new
            {
                result.ErrorField, 
                result.Message
            });
        }
        
        return Ok(result.Value);
    }
    
    [HttpPost("vote")]
    public async Task<ActionResult> Vote([FromBody] CommentUpdateVoteDto request)
    {
        var result = await commentService.UpdateCommentsVote(request);
    
        if (!result.IsSuccess) return BadRequest(result.Message);
        return Ok(result.Value);
    }
    
    [HttpDelete("delete/{id:guid}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    public async Task<ActionResult> Delete([FromRoute]Guid id)
    {
        var response = await commentService.DeleteComment(id);
        if (!response.IsSuccess) return NotFound(response.Message);

        return Ok();
    }
}