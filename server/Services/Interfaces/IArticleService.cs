using DTOs.News;
using Services.Types;

namespace Services.Interfaces;

public interface IArticleService
{
    Task<ResponseResult<ArticleDetailDto>> GetArticleById(Guid id);
    Task<ResponseResult<ArticleDetailDto>> GetArticleBySlug(string slug);
    Task<ResponseResult<PagedResult<ArticleListDto>>> ListArticles(int page, int pageSize, string? tag = null);
    Task<ResponseResult<PagedResult<ArticleListDto>>> ListAllSummary(int page, int pageSize, string? tag = null);
    Task<ResponseResult<List<ArticleListDto>>> GetRecentArticles(int count, string? tag = null);
    Task<ResponseResult<bool>> CreateArticle(ArticleCreateDto dto);
    Task<ResponseResult<bool>> UpdateArticle(ArticleUpdateDto dto);
    Task<ResponseResult<bool>> DeleteArticle(Guid id);
}