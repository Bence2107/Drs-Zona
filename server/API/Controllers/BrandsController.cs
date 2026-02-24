using DTOs.Standings;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Drs_Zona.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BrandController(IBrandService brandService) : ControllerBase
{
    [HttpGet("get/{id:guid}")]
    public async Task<ActionResult<BrandDetailDto>> Get([FromRoute] Guid id)
    {
        var response = await brandService.GetBrandById(id);
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
    public async Task<ActionResult<BrandDetailDto>> Get([FromRoute] string name)
    {
        var response = await brandService.GetBrandByName(name);
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
    public async Task<ActionResult<BrandListDto>> GetAll()
    {
        var response = await brandService.ListBrands();
        return Ok(response);
    }
    
    [HttpPost("create")]
    public async Task<ActionResult> Create([FromBody]BrandCreateDto dto)
    {
        var result = await brandService.CreateBrand(dto);

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
    public async Task<ActionResult> Update([FromBody]BrandUpdateDto dto)
    {
        var result = await brandService.UpdateBrand(dto);
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
        var response = await brandService.DeleteBrand(id);
        if (!response.IsSuccess) return NotFound(response.Message);

        return Ok();
    }
}