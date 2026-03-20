using DTOs.Standings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Drs_Zona.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConstructorsController(IConstructorsService constructorsService): ControllerBase 
{
    [Authorize(Policy = "EditorOrAdmin")]
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
    
    [Authorize(Policy = "EditorOrAdmin")]
    [HttpGet("getAllConstructors")]
    public async Task<ActionResult<List<ConstructorListDto>>> GetAllConstructors()
    {
        var response = await constructorsService.GetAllConstructors();
        if (!response.IsSuccess)
            return BadRequest(new { response.ErrorField, response.Message });
        return Ok(response.Value);
    }
    
    [Authorize(Policy = "EditorOrAdmin")]
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
    
    [Authorize(Policy = "EditorOrAdmin")]
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