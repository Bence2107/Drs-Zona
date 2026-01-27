using DTOs.News;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Drs_Zona.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticleController(IArticleService articleService): ControllerBase
{
    [HttpGet("get/{id:guid}")]
    [ProducesResponseType(typeof(ArticleDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Get([FromRoute]Guid id)
    {
        var response = articleService.GetArticleById(id);
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
    [ProducesResponseType(typeof(List<ArticleListDto>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var response = articleService.ListArticles();
        return Ok(response);
    }

    [HttpGet("getRecent/{number:int}")]
    [ProducesResponseType(typeof(List<ArticleListDto>), StatusCodes.Status200OK)]
    public IActionResult GetRecent([FromRoute]int number)
    {
        var response  = articleService.GetRecentArticles(number);
        return Ok(response);
    }
    
    [HttpPost("create/{authorId:guid}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    public IActionResult Create([FromBody]ArticleCreateDto dto, [FromRoute]Guid authorId)
    {
        var result = articleService.CreateArticle(dto, authorId);

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
    public IActionResult Update([FromBody]ArticleUpdateDto dto)
    {
        var result = articleService.UpdateArticle(dto);
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
        var articleResponse = articleService.GetArticleById(id);
        if (!articleResponse.IsSuccess)
        {
            return BadRequest(new
            {
                articleResponse.ErrorField, 
                articleResponse.Message
            });
        }
        
        var result = articleService.DeleteArticle(id);
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