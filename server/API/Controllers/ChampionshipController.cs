using DTOs.Standings;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Drs_Zona.API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class ChampionshipController(IChampionshipService championshipService): ControllerBase 
{
    [HttpGet("getAllChampionshipsBySeries/{seriesId:guid}")]
    public async Task<ActionResult<List<ChampionshipRowDto>>> GetAllChampionshipsBySeries(Guid seriesId)
    {
        var response = await championshipService.GetAllChampionshipsBySeries(seriesId);
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
        var response = await championshipService.GetConstructorsBySeason(constChampId);
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
        var response = await championshipService.GetDriversBySeason(driverChampId);
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
        var response = await championshipService.GetGrandsPrixByChampionship(driverChampId);
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
    
    [HttpGet("getParticipations/{driversChampId:guid}/{constructorsChampId:guid}")]
    public async Task<ActionResult<ParticipationListDto>> GetParticipations(Guid driversChampId, Guid constructorsChampId)
    {
        var response = await championshipService.GetParticipations(driversChampId, constructorsChampId);
        if (!response.IsSuccess)
            return BadRequest(new { response.ErrorField, response.Message });
        return Ok(response.Value);
    }
    
    [HttpGet("getSeasonsBySeries/{seriesId:guid}")]
    public async Task<ActionResult<List<YearLookupDto>>> GetSeasonsBySeries(Guid seriesId)
    {
        var response = await championshipService.GetSeasonsBySeries(seriesId);
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
    
     [HttpPost("createChampionship")]
    public async Task<ActionResult> CreateChampionship([FromBody] ChampionshipCreateDto dto)
    {
        var response = await championshipService.CreateChampionship(dto);
        if (!response.IsSuccess)
            return BadRequest(new { response.ErrorField, response.Message });
        return Ok();
    }
    
    [HttpPost("updateChampionshipStatus/{driversChampId:guid}/{constructorsChampId:guid}/{status}")]
    public async Task<ActionResult> UpdateChampionshipStatus(Guid driversChampId, Guid constructorsChampId, string status)
    {
        var response = await championshipService.UpdateChampionshipStatus(driversChampId, constructorsChampId, status);
        if (!response.IsSuccess)
            return BadRequest(new { response.ErrorField, response.Message });
        return Ok();
    }
    
    [HttpPost("addParticipations")]
    public async Task<ActionResult> AddParticipations([FromBody] ParticipationAddDto dto)
    {
        var response = await championshipService.CreateParticipations(dto);
        if (!response.IsSuccess)
            return BadRequest(new { response.ErrorField, response.Message });
        return Ok();
    }

    [HttpDelete("removeDriverParticipation/{driverId:guid}/{driversChampId:guid}")]
    public async Task<ActionResult> RemoveDriverParticipation(Guid driverId, Guid driversChampId)
    {
        var response = await championshipService.DeleteDriverParticipation(driverId, driversChampId);
        if (!response.IsSuccess)
            return BadRequest(new { response.ErrorField, response.Message });
        return Ok();
    }

    [HttpDelete("removeConstructorCompetition/{constructorId:guid}/{constructorsChampId:guid}")]
    public async Task<ActionResult> RemoveConstructorCompetition(Guid constructorId, Guid constructorsChampId)
    {
        var response = await championshipService.DeleteConstructorCompetition(constructorId, constructorsChampId);
        if (!response.IsSuccess)
            return BadRequest(new { response.ErrorField, response.Message });
        return Ok();
    }
}