using DTOs.News;

namespace Services.Interfaces;

public interface IArticleService
{
    ResponseResult<ArticleDetailDto> GetArticleById(Guid id);
    ResponseResult<List<ArticleListDto>> ListArticles();
    ResponseResult<List<ArticleListDto>> GetRecentArticles(int count);
    ResponseResult<bool> CreateArticle(ArticleCreateDto dto);
    ResponseResult<bool> UpdateArticle(ArticleUpdateDto dto);
    ResponseResult<bool> DeleteArticle(Guid id);
}