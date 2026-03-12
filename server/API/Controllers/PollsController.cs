using DTOs.Polls;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Drs_Zona.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PollController(IPollService pollService): ControllerBase
{
    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var guid) ? guid : null;
    }
    
    [HttpGet("get/{id:guid}")]
    public async Task<ActionResult<PollDto>> GetById([FromRoute] Guid id)
    {
        var userId = GetCurrentUserId();
        var response = await pollService.GetPollById(id, userId);
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
    
    [HttpGet("getByCreatorId/{id:guid}")]
    public async Task<ActionResult<List<PollListDto>>> GetByCreatorId([FromRoute] Guid id,  [FromQuery] string? tag = null)
    {
        var response = await pollService.GetPollByCreatorId(id, tag);
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
    
    [HttpGet("getAll")]
    public async Task<ActionResult<List<PollListDto>>> GetAll([FromQuery] string? tag = null)
    {
        var response = await pollService.ListAllPolls(tag);
        return Ok(response);
    }
    
    [HttpGet("getAllActive")]
    public async Task<ActionResult<List<PollListDto>>> GetAllActive([FromQuery] string? tag = null)
    {
        var response = await pollService.GetActivePolls(tag);
        return Ok(response);
    }
    
    [HttpGet("getAllExpired")]
    public async Task<ActionResult<List<PollListDto>>> GetAllExpired([FromQuery] string? tag = null)
    {
        var response = await pollService.GetExpiredPolls(tag);
        return Ok(response);
    }
    
    [HttpPost("create/{userId:guid}")]
    public async Task<ActionResult> Create([FromBody]PollCreateDto dto)
    {
        var userId = GetCurrentUserId();
        var result = await pollService.Create(dto, userId);

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
    
    [HttpPost("vote/{pollId:guid}/{pollOptionId:guid}/{userId:guid}")]
    public async Task<ActionResult> Vote([FromRoute]Guid pollId, [FromRoute]Guid pollOptionId, [FromRoute] Guid userId)
    {
        var result = await pollService.Vote(pollId, pollOptionId, userId);

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
    
    [HttpPost("removeVote/{pollId:guid}/{pollOptionId:guid}/{userId:guid}")]
    public async Task<ActionResult> RemoveVote([FromRoute]Guid pollId, [FromRoute]Guid pollOptionId, [FromRoute] Guid userId)
    {
        var result = await pollService.RemoveVote(pollId, pollOptionId, userId);

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

    [HttpDelete("delete/{id:guid}")]
    public async Task<ActionResult> Delete([FromRoute]Guid id)
    {
        var userId = GetCurrentUserId();
        var response = await pollService.Delete(id, userId);
        if (!response.IsSuccess) return NotFound(response.Message);

        return Ok();
    }
}