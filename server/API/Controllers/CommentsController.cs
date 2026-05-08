using DTOs.News;
using Microsoft.AspNetCore.Authorization;
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
    
    [Authorize]
    [HttpGet("getUsersComments")]
    public async Task<ActionResult<List<CommentDetailDto>>> GetUsersComments()
    {
        var userId = GetCurrentUserId();
        
        var response = await commentService.GetUsersComments(userId!.Value);
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
    
    [Authorize]
    [HttpPost("create")]
    public async Task<ActionResult> Create([FromBody]CommentCreateDto dto)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
            return Unauthorized();
        
        var result = await commentService.Create(dto, userId.Value);

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
    
    [Authorize]
    [HttpPost("updateContent")]
    public async Task<ActionResult> UpdateContent([FromBody]CommentContentUpdateDto commentContentUpdateDto)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Unauthorized();
        }
        
        var result = await commentService.UpdateCommentsContent(commentContentUpdateDto, userId.Value);
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
    
    [Authorize]
    [HttpPost("vote")]
    public async Task<ActionResult> Vote([FromBody] CommentUpdateVoteDto request)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }
        
        var result = await commentService.UpdateCommentsVote(request, userId.Value);
    
        if (!result.IsSuccess) return BadRequest(result.Message);
        return Ok(result.Value);
    }
    
    [Authorize]
    [HttpDelete("delete/{commentId:guid}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    public async Task<ActionResult> Delete([FromRoute]Guid commentId)
    {
        var userId = GetCurrentUserId();
        
        var response = await commentService.DeleteComment(commentId, userId!.Value);
        if (!response.IsSuccess) return NotFound(response.Message);

        return Ok();
    }
}