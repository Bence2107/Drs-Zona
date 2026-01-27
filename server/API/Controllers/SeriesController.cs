using DTOs.Standings;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Drs_Zona.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeriesController(ISeriesService seriesService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetSeriesById([FromRoute]Guid id)
    {
        var response = seriesService.GetSeriesById(id);
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
    public IActionResult GetSeriesByName([FromRoute]string name)
    {
        var response = seriesService.GetSeriesByName(name);
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

    [HttpGet]
    public IActionResult GetAllSeries()
    {
        var response = seriesService.ListSeries();
        return Ok(response.Value);
    }

    [HttpPost("create")]
    public IActionResult Create([FromBody]SeriesCreateDto dto)
    {
        var response = seriesService.CreateSeries(dto);
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
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    public IActionResult Update([FromBody]SeriesUpdateDto dto)
    {
        var response = seriesService.Update(dto);
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
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    public IActionResult Delete(Guid id)
    {
        var response = seriesService.Delete(id);
        if (!response.IsSuccess) return NotFound(response.Message);

        return Ok();
    }
}