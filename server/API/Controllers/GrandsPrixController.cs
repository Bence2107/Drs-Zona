using DTOs.RaceTracks;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Drs_Zona.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GrandPrixController (IGrandPrixService grandPrixService): ControllerBase 
{
    [HttpGet("get/{grandPrixId:guid}")]
    public async Task<ActionResult<GrandPrixDetailDto>> GetGrandPrixById([FromRoute] Guid grandPrixId)
    {
        var response = await grandPrixService.GetGrandPrixById(grandPrixId);
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

    [HttpGet("getAllCircuits")]
    public async Task<ActionResult<CircuitListDto>> GetAllCircuits()
    {
        var response = await grandPrixService.GetAllCircuits();
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
    
    [HttpGet("getSeasonGrandsPrix/{seriesId:guid}/{year:int}")]
    public async Task<ActionResult<GrandPrixDetailDto>> GetSeasonGrandsPrix([FromRoute] Guid seriesId, [FromRoute] int year)
    {
        var response = await grandPrixService.GetSeasonGrandPrixList(seriesId, year);
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
    
    [HttpPost("create")]
    public async Task<ActionResult> Create([FromBody]GrandPrixCreateDto dto)
    {
        var result = await grandPrixService.Create(dto);

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
    
    [HttpPost("update")]
    public async Task<ActionResult> Update([FromBody]GrandPrixUpdateDto dto)
    {
        var result = await grandPrixService.Update(dto);
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
}