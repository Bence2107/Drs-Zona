using DTOs.Standings;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Drs_Zona.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DriversController(IDriverService driverService): ControllerBase
{
    [HttpGet("get/{id:guid}")]
    public async Task<ActionResult<DriverDetailDto>> Get([FromRoute]Guid id)
    {
        var response = await driverService.GetDriverById(id);
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
    
    [HttpGet("getAllByChampionship/{championshipId:guid}")]
    public async Task<ActionResult<List<DriverDetailDto>>> GetAll(Guid championshipId)
    {
        var response = await driverService.ListAllDriversByChampionships(championshipId);
        return Ok(response.Value);
    }

    [HttpPost("create")]
    public async Task<ActionResult> Create([FromBody] DriverCreateDto dto)
    {
        var result = await driverService.CreateDriver(dto);

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
    public async Task<ActionResult> Update([FromBody]DriverUpdateDto dto)
    {
        var result = await driverService.UpdateDriver(dto);
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
        var driverResponse = await driverService.GetDriverById(id);
        if (!driverResponse.IsSuccess)
        {
            return BadRequest(new
            {
                driverResponse.ErrorField, 
                driverResponse.Message
            });
        }

        var result = await driverService.DeleteDriver(id);
        if (!result.IsSuccess)
        {
            return BadRequest(new
            {
                result.ErrorField, 
                result.Message
            });
        }
        return Ok();
    }
}