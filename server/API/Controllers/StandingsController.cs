using DTOs.Standings;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Drs_Zona.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StandingsController(IStandingsService standingsService): ControllerBase
{
    [HttpGet("getDefaultFilters")]
    public async Task<ActionResult<DefaultFiltersDto>> GetDefaultFilters()
    {
        var response = await standingsService.GetDefaultFilters();
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

    [HttpGet("getDriversByDriversChampionship/{driverChampId:guid}")]
    public async Task<ActionResult<List<DriverLookUpDto>>> GetDriversByDriversChamp(Guid driverChampId)
    {
        var response = await standingsService.GetDriversBySeason(driverChampId);
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
    
    [HttpGet("getConstructorsByConstructorsChampionship/{constChampId:guid}")]
    public async Task<ActionResult<List<ConstructorLookUpDto>>> GetConstructorsByConstructorsChamp(Guid constChampId)
    {
        var response = await standingsService.GetConstructorsBySeason(constChampId);
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

    [HttpGet("getGrandPrixByChampionship/{driverChampId:guid}")]
    public async Task<ActionResult<List<GrandPrixLookupDto>>> GetGrandsPrixByChampionship(Guid driverChampId)
    {
        var response = await standingsService.GetGrandsPrixByChampionship(driverChampId);
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
}