using DTOs.Standings;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Drs_Zona.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BrandController(IBrandService brandService) : ControllerBase
{
    [HttpGet("getAll")]
    public async Task<ActionResult<BrandListDto>> GetAll()
    {
        var response = await brandService.ListBrands();
        return Ok(response);
    }
}