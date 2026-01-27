using DTOs.Standings;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Drs_Zona.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BrandController(IBrandService brandService) : ControllerBase
{
    [HttpGet("get/{id:guid}")]
    [ProducesResponseType(typeof(BrandDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Get([FromRoute] Guid id)
    {
        var response = brandService.GetBrandById(id);
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
    
    [HttpGet("get/{name}")]
    [ProducesResponseType(typeof(BrandDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Get([FromRoute] string name)
    {
        var response = brandService.GetBrandByName(name);
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
    
    [HttpGet("getAll")]
    [ProducesResponseType(typeof(List<BrandListDto>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var response = brandService.ListBrands();
        return Ok(response);
    }
    
    [HttpPost("create")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    public IActionResult Create([FromBody]BrandCreateDto dto)
    {
        var result = brandService.CreateBrand(dto);

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
    public IActionResult Update([FromBody]BrandUpdateDto dto)
    {
        var result = brandService.UpdateBrand(dto);
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
        var response = brandService.DeleteBrand(id);
        if (!response.IsSuccess) return NotFound(response.Message);

        return Ok();
    }
}