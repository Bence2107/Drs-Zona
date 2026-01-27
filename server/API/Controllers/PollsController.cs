using DTOs.Polls;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Drs_Zona.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PollController(IPollService pollService): ControllerBase
{
    [HttpGet("get/{id:guid}")]
    [ProducesResponseType(typeof(PollDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById([FromRoute] Guid id)
    {
        var response = pollService.GetPollById(id);
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
    [ProducesResponseType(typeof(List<PollListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetByCreatorId([FromRoute] Guid id)
    {
        var response = pollService.GetPollByCreatorId(id);
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
    [ProducesResponseType(typeof(List<PollListDto>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var response = pollService.ListAllPolls();
        return Ok(response);
    }
    
    [HttpGet("getAllActive")]
    [ProducesResponseType(typeof(List<PollListDto>), StatusCodes.Status200OK)]
    public IActionResult GetAllActive()
    {
        var response = pollService.GetActivePolls();
        return Ok(response);
    }
    
    [HttpGet("getAllExpired")]
    [ProducesResponseType(typeof(List<PollListDto>), StatusCodes.Status200OK)]
    public IActionResult GetAllExpired()
    {
        var response = pollService.GetExpiredPolls();
        return Ok(response);
    }
    
    [HttpPost("create/{userId:guid}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    public IActionResult Create([FromBody]PollCreateDto dto, Guid userId)
    {
        var result = pollService.Create(dto, userId);

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
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    public IActionResult Vote([FromRoute]Guid pollId, [FromRoute]Guid pollOptionId, [FromRoute] Guid userId)
    {
        var result = pollService.Vote(pollId, pollOptionId, userId);

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
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    public IActionResult RemoveVote([FromRoute]Guid pollId, [FromRoute]Guid pollOptionId, [FromRoute] Guid userId)
    {
        var result = pollService.RemoveVote(pollId, pollOptionId, userId);

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
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    public IActionResult Delete([FromRoute]Guid id, [FromQuery]Guid userId)
    {
        var response = pollService.Delete(id, userId);
        if (!response.IsSuccess) return NotFound(response.Message);

        return Ok();
    }
}