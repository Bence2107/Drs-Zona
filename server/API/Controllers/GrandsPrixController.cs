using DTOs.RaceTracks;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Drs_Zona.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GrandPrixController (IGrandPrixService grandPrixService): ControllerBase
{
    [HttpGet("get/{grandPrixId:int}")]
    [ProducesResponseType(typeof(GrandPrixDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetGrandPrixById([FromRoute] int grandPrixId)
    {
        var response = grandPrixService.GetGrandPrixById(grandPrixId);
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
    
    [HttpGet("getSeasonGrandsPrix/{seriesId:int}/{year:int}")]
    [ProducesResponseType(typeof(GrandPrixDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetSeasonGrandsPrix([FromRoute] int seriesId, [FromRoute] int year)
    {
        var response = grandPrixService.GetSeasonGrandPrixList(seriesId, year);
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
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    public IActionResult Create([FromBody]GrandPrixCreateDto dto)
    {
        var result = grandPrixService.CreateGrandPrix(dto);

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
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    public IActionResult Update([FromBody]GrandPrixUpdateDto dto)
    {
        var result = grandPrixService.UpdateGrandPrix(dto);
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
        var response = grandPrixService.DeleteGrandPrix(id);
        if (!response.IsSuccess) return NotFound(response.Message);

        return Ok();
    }
}