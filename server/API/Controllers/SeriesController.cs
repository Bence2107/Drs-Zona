using DTOs.Standings;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Drs_Zona.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeriesController(ISeriesService seriesService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SeriesDetailDto>> GetSeriesById([FromRoute]Guid id)
    {
        var response = await seriesService.GetSeriesById(id);
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

    [HttpPost("create")]
    public async Task<ActionResult> Create([FromBody]SeriesCreateDto dto)
    {
        var response = await seriesService.CreateSeries(dto);
        if (!response.IsSuccess)
        {
            return BadRequest(new
            {
                response.ErrorField, 
                response.Message
            });
        }
        return Ok(new { Message = "Series created successfully" });
    }

    [HttpPost("update")]
    public async Task<ActionResult> Update([FromBody]SeriesUpdateDto dto)
    {
        var response = await seriesService.Update(dto);
        if (!response.IsSuccess)
        {
          
            return BadRequest(new
            {
                response.ErrorField, 
                response.Message
            });
        }

        return Ok(); 
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var response = await seriesService.Delete(id);
        if (!response.IsSuccess) return NotFound(response.Message);

        return Ok();
    }
}