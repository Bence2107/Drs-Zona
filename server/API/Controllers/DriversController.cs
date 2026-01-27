using DTOs.Standings;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Drs_Zona.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DriversController(IDriverService driverService): ControllerBase
{
    [HttpGet("get/{id:guid}")]
    [ProducesResponseType(typeof(DriverDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Get([FromRoute]Guid id)
    {
        var response = driverService.GetDriverById(id);
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
    [ProducesResponseType(typeof(List<DriverDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetAll(Guid championshipId)
    {
        var response = driverService.ListAllDriversByChampionships(championshipId);
        return Ok(response.Value);
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Create([FromBody] DriverCreateDto dto)
    {
        var result = driverService.CreateDriver(dto);

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
    public IActionResult Update([FromBody]DriverUpdateDto dto)
    {
        var result = driverService.UpdateDriver(dto);
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
    public IActionResult Delete([FromRoute]Guid id)
    {
        var driverResponse = driverService.GetDriverById(id);
        if (!driverResponse.IsSuccess)
        {
            return BadRequest(new
            {
                driverResponse.ErrorField, 
                driverResponse.Message
            });
        }

        var result = driverService.DeleteDriver(id);
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