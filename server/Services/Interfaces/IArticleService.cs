using DTOs.News;

namespace Services.Interfaces;

public interface IArticleService
{
    Task<ResponseResult<ArticleDetailDto>> GetArticleById(Guid id);
    Task<ResponseResult<ArticleDetailDto>> GetArticleBySlug(string slug);
    Task<ResponseResult<List<ArticleListDto>>> ListArticles();
    Task<ResponseResult<List<ArticleListDto>>> ListAllSummary();
    Task<ResponseResult<List<ArticleListDto>>> GetRecentArticles(int count);
    Task<ResponseResult<bool>> CreateArticle(ArticleCreateDto dto);
    Task<ResponseResult<bool>> UpdateArticle(ArticleUpdateDto dto);
    Task<ResponseResult<bool>> DeleteArticle(Guid id);
}