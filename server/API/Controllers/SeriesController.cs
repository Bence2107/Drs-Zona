using DTOs.Standings;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Drs_Zona.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeriesController(ISeriesService seriesService) : ControllerBase 
{
    [HttpGet("name/{name}")]
    public async Task<ActionResult<SeriesDetailDto>> GetSeriesByName([FromRoute]string name)
    {
        var response = await seriesService.GetSeriesByName(name);
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

    [HttpGet("getAllSeries")]
    public async Task<ActionResult<List<SeriesListDto>>> GetAllSeries()
    {
        var response = await seriesService.ListSeries();
        return Ok(response.Value);
    }
}