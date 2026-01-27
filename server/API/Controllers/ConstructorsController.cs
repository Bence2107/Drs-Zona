using DTOs.Standings;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Drs_Zona.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConstructorsController(IConstructorsService constructorsService): ControllerBase
{
    [HttpGet("get/{id:guid}")]
    [ProducesResponseType(typeof(ConstructorDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Get([FromRoute]Guid id)
    {
        var response = constructorsService.GetById(id);
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
    [ProducesResponseType(typeof(List<ConstructorListDto>), StatusCodes.Status201Created)]
    public IActionResult GetAll([FromRoute]Guid championshipId)
    {
        var response = constructorsService.ListAllConstructorsByChampionship(championshipId);
        return Ok(response.Value);
    }
    
    [HttpPost("create")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Create([FromBody] ConstructorCreateDto dto)
    {
        var result = constructorsService.CreateConstructor(dto);

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
    public IActionResult Update([FromBody]ConstructorUpdateDto dto)
    {
        var result = constructorsService.UpdateConstructor(dto);
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