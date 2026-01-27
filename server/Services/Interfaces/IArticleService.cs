using DTOs.News;

namespace Services.Interfaces;

public interface IArticleService
{
    ResponseResult<ArticleDetailDto> GetArticleById(int id);
    ResponseResult<List<ArticleListDto>> ListArticles();
    ResponseResult<List<ArticleListDto>> GetRecentArticles(int count);
    ResponseResult<bool> CreateArticle(ArticleCreateDto dto, int authorId);
    ResponseResult<bool> UpdateArticle(ArticleUpdateDto dto);
    ResponseResult<bool> DeleteArticle(int id);
}