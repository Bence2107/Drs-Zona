using DTOs.Standings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Drs_Zona.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BrandsController(IBrandService brandService) : ControllerBase 
{
    [Authorize(Policy = "EditorOrAdmin")]
    [HttpGet("getAll")]
    public async Task<ActionResult<BrandListDto>> GetAll()
    {
        var response = await brandService.ListBrands();
        return Ok(response);
    }
}