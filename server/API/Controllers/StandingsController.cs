using DTOs.Standings;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Drs_Zona.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StandingsController(IStandingsService standingsService): ControllerBase 
{
    [HttpGet("getAllSeries")]
    public async Task<ActionResult<List<SeriesLookupDto>>> GetAllSeries()
    {
        var response = await standingsService.GetAllSeries();
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
    
    [HttpGet("getByDriversChampionshipId/{driversChampId:guid}")]
    public async Task<ActionResult<DriverStandingsDto>> GetDriverStandings(Guid driversChampId)
    {
        var response = await standingsService.GetDriverStandings(driversChampId);
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

    [HttpGet("getByConstructorChampionshipId/{constructsChampId:guid}")]
    public async Task<ActionResult<ConstructorStandingsDto>> GetConstructorStandings(Guid constructsChampId)
    {
        var response = await standingsService.GetConstructorStandings(constructsChampId);
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
    
    [HttpGet("getConstructorsResultsBySeason/{constructorId:guid}/{constructorChampId:guid}")]
    public async Task<ActionResult<List<ConstructorSeasonResultDto>>> GetConstructorsResultsBySeason(Guid constructorId, Guid constructorChampId)
    {
        var response = await standingsService.GetConstructorResultsBySeason(constructorId, constructorChampId);
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
    
    [HttpGet("getDriverResultsBySeason/{driverId:guid}/{driverChampId:guid}")]
    public async Task<ActionResult<List<DriverSeasonResultDto>>> GetDriverResultsBySeason(Guid driverId, Guid driverChampId)
    {
        var response = await standingsService.GetDriverResultsBySeason(driverId, driverChampId);
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
    
    [HttpGet("getGrandPrixResults/{grandPrixId:guid}/{session}")]
    public async Task<ActionResult<GrandPrixResultsDto>> GetGrandPrixResults(Guid grandPrixId, string session)
    {
        var response = await standingsService.GetGrandPrixResults(grandPrixId, session);
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
    
    [HttpGet("getSeasonOverview/{driverChampId:guid}")]
    public async Task<ActionResult<List<SeasonOverviewDto>>> GetSeasonOverview(Guid driverChampId)
    {
        var response = await standingsService.GetSeasonOverview(driverChampId);
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
    
    [HttpGet("getSeasonsBySeries/{seriesId:guid}")]
    public async Task<ActionResult<List<YearLookupDto>>> GetSeasonsBySeries(Guid seriesId)
    {
        var response = await standingsService.GetSeasonsBySeries(seriesId);
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
    
    [HttpGet("getSessionForEdit/{grandPrixId:guid}/{session}")]
    public async Task<ActionResult<SessionEditDto>> GetSessionForEdit(Guid grandPrixId, string session)
    {
        var response = await standingsService.GetSessionForEdit(grandPrixId, session);
        if (!response.IsSuccess)
            return BadRequest(new { response.ErrorField, response.Message });
        return Ok(response.Value);
    }
    
    [HttpGet("getSessionsByGrandPrix/{grandPrixId:guid}")]
    public async Task<ActionResult<List<string>>> GetSessionsByGrandPrix(Guid grandPrixId)
    {
        var response = await standingsService.GetSessionsByGrandPrix(grandPrixId);
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

    [HttpPost("insertResults")]
    public async Task<ActionResult> InsertResults([FromBody]BatchResultCreateDto dto)
    {
        var response = await standingsService.InsertResults(dto);
        if (!response.IsSuccess)
            return BadRequest(new { response.ErrorField, response.Message });
        return Ok();
    }
    
    [HttpPost("saveSessionResults")]
    public async Task<ActionResult> SaveSessionResults([FromBody]BatchResultCreateDto dto)
    {
        var response = await standingsService.SaveSessionResults(dto);
        if (!response.IsSuccess)
            return BadRequest(new { response.ErrorField, response.Message });
        return Ok();
    }
    
    [HttpGet("getGrandPrixContext/{grandPrixId:guid}")]
    public async Task<ActionResult<GrandPrixChampionshipContextDto>> GetGrandPrixContext(Guid grandPrixId)
    {
        var response = await standingsService.GetGrandPrixContext(grandPrixId);
        if (!response.IsSuccess)
            return BadRequest(new { response.ErrorField, response.Message });
        return Ok(response.Value);
    }
    
    [HttpPost("updateSingleResult")]
    public async Task<ActionResult> UpdateSingleResult([FromBody] SingleResultUpdateDto dto)
    {
        var response = await standingsService.UpdateSingleResult(dto);
        if (!response.IsSuccess)
            return BadRequest(new { response.ErrorField, response.Message });
        return Ok();
    }
    
    [HttpPost("recalculateSession/{grandPrixId:guid}/{session}")]
    public async Task<ActionResult> RecalculateSession(Guid grandPrixId, string session)
    {
        var response = await standingsService.RecalculateSession(grandPrixId, session);
        if (!response.IsSuccess)
            return BadRequest(new { response.ErrorField, response.Message });
        return Ok();
    }
}