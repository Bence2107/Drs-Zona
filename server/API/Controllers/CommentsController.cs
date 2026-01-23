using DTOs.News;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Drs_Zona.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController(ICommentService commentService): ControllerBase
{
    [HttpGet("getCommentsWithoutReplies/{articleId:int}")]
    [ProducesResponseType(typeof(List<CommentDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetCommentsWithoutReplies([FromRoute] int articleId)
    {
        var response = commentService.GetArticleCommentsWithoutReplies(articleId);
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
    
    [HttpGet("getCommentReplies/{commentId:int}")]
    [ProducesResponseType(typeof(List<CommentDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetCommentReplies([FromRoute] int commentId)
    {
        var response = commentService.GetCommentReplies(commentId);
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
    
    [HttpGet("getUsersComments/{userId:int}")]
    [ProducesResponseType(typeof(List<CommentDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetUsersComments([FromRoute] int userId)
    {
        var response = commentService.GetUsersComments(userId);
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
    
    [HttpPost("create/{userId:int}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    public IActionResult Create([FromBody]CommentCreateDto dto, [FromRoute]int userId)
    {
        var result = commentService.AddComment(dto, userId);

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
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    public IActionResult UpdateContent([FromBody]CommentContentUpdateDto commentUpdateVoteDto)
    {
        var result = commentService.UpdateCommentsContent(commentUpdateVoteDto);
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
    
    [HttpPost("updateVote")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    public IActionResult UpdateContent([FromBody]CommentUpdateVoteDto commentUpdateVoteDto)
    {
        var result = commentService.UpdateCommentsVote(commentUpdateVoteDto);
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
    
    [HttpDelete("delete/{id:int}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    public IActionResult Delete([FromRoute]int id)
    {
        var response = commentService.DeleteComment(id);
        if (!response.IsSuccess) return NotFound(response.Message);

        return Ok();
    }
}