using DTOs.Standings;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Drs_Zona.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StandingsController(IStandingsService standingsService): ControllerBase
{
    [HttpGet("getDefaultFilters")]
    [ProducesResponseType(typeof(DefaultFiltersDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetDefaultFilters()
    {
        var response = standingsService.GetDefaultFilters();
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
    [ProducesResponseType(typeof(List<SeriesLookupDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetAllSeries()
    {
        var response = standingsService.GetAllSeries();
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
    [ProducesResponseType(typeof(List<YearLookupDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetSeasonsBySeries(Guid seriesId)
    {
        var response = standingsService.GetSeasonsBySeries(seriesId);
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
    [ProducesResponseType(typeof(List<GrandPrixLookupDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetGrandsPrixByChampionship(Guid driverChampId)
    {
        var response = standingsService.GetGrandsPrixByChampionship(driverChampId);
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

    [HttpGet("getSeasonsByGrandPrix/{grandPrixId:guid}")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetSessionsByGrandPrix(Guid grandPrixId)
    {
        var response = standingsService.GetSessionsByGrandPrix(grandPrixId);
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
    [ProducesResponseType(typeof(DriverStandingsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetDriverStandings(Guid driversChampId)
    {
        var response = standingsService.GetDriverStandings(driversChampId);
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
    [ProducesResponseType(typeof(ConstructorStandingsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetConstructorStandings(Guid constructsChampId)
    {
        var response = standingsService.GetConstructorStandings(constructsChampId);
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
    [ProducesResponseType(typeof(GrandPrixResultsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetGrandPrixResults(Guid grandPrixId, string session)
    {
        var response = standingsService.GetGrandPrixResults(grandPrixId, session);
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
    [ProducesResponseType(typeof(List<DriverSeasonResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetDriverResultsBySeason(Guid driverId, Guid driverChampId)
    {
        var response = standingsService.GetDriverResultsBySeason(driverId, driverChampId);
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
    [ProducesResponseType(typeof(List<ConstructorSeasonResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetConstructorsResultsBySeason(Guid constructorId, Guid constructorChampId)
    {
        var response = standingsService.GetConstructorResultsBySeason(constructorId, constructorChampId);
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
    [ProducesResponseType(typeof(List<ConstructorSeasonResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetSeasonOverview(Guid driverChampId)
    {
        var response = standingsService.GetSeasonOverview(driverChampId);
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