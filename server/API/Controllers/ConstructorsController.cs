using DTOs.Standings;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Drs_Zona.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConstructorsController(IConstructorsService constructorsService): ControllerBase
{
    [HttpGet("get/{id:guid}")]
    public async Task<ActionResult<ConstructorDetailDto>> Get([FromRoute]Guid id)
    {
        var response = await constructorsService.GetById(id);
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
    public async Task<ActionResult<List<ConstructorListDto>>> GetAll([FromRoute]Guid championshipId)
    {
        var response = await constructorsService.ListAllConstructorsByChampionship(championshipId);
        return Ok(response.Value);
    }
    
    [HttpGet("getAllConstructors")]
    public async Task<ActionResult<List<ConstructorListDto>>> GetAllConstructors()
    {
        var response = await constructorsService.GetAllConstructors();
        if (!response.IsSuccess)
            return BadRequest(new { response.ErrorField, response.Message });
        return Ok(response.Value);
    }
    
    [HttpPost("create")]
    public async Task<ActionResult> Create([FromBody] ConstructorCreateDto dto)
    {
        var result = await constructorsService.Create(dto);

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
    public async Task<ActionResult> Update([FromBody]ConstructorUpdateDto dto)
    {
        var result = await constructorsService.Update(dto);
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