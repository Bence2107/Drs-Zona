using DTOs.News;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Types;

namespace Drs_Zona.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticleController(IArticleService articleService): ControllerBase
{
    [HttpGet("get/{id:guid}")]
    public async Task<ActionResult<ArticleDetailDto>> Get([FromRoute]Guid id)
    {
        var response = await articleService.GetArticleById(id);
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
    
    [HttpGet("get/{slug}")]
    public async Task<ActionResult<ArticleDetailDto>> GetBySlug([FromRoute]string slug)
    {
        var response = await articleService.GetArticleBySlug(slug);
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
    
    [HttpGet("getAllArticles")]
    public async Task<ActionResult<PagedResult<ArticleListDto>>> GetAllArticles(int page, int pageSize, string? tag)
    {
        var response = await articleService.ListArticles(page, pageSize, tag);
        return Ok(response);
    }
    
    [HttpGet("getAllSummary")]
    public async Task<ActionResult<PagedResult<ArticleListDto>>> GetAllSummary(int page, int pageSize, string? tag)
    {
        var response = await articleService.ListAllSummary(page, pageSize, tag);
        return Ok(response);
    }

    [HttpGet("getRecent/{number:int}")]
    public async Task<ActionResult<List<ArticleListDto>>> GetRecent([FromRoute]int number, [FromQuery] string? tag)
    {
        var response  = await articleService.GetRecentArticles(number, tag);
        return Ok(response);
    }
    
    [HttpPost("create")]
    public async Task<ActionResult> Create([FromBody]ArticleCreateDto dto)
    {
        var result = await articleService.Create(dto);

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
    public async Task<ActionResult> Update([FromBody]ArticleUpdateDto dto)
    {
        var result = await articleService.Update(dto);
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
        var articleResponse = await articleService.GetArticleById(id);
        if (!articleResponse.IsSuccess)
        {
            return BadRequest(new
            {
                articleResponse.ErrorField, 
                articleResponse.Message
            });
        }
        
        var result = await articleService.Delete(id);
        if (!result.IsSuccess)
        {
            return BadRequest(new
            {
                result.ErrorField, 
                result.Message
            });
        }
        return Ok();
    }
}