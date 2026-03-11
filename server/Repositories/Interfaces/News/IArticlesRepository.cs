using Entities.Models.News;

namespace Repositories.Interfaces.News;

public interface IArticlesRepository
{
    Task<Article?> GetArticleById(Guid id);
    Task<Article?> GetArticleBySlug(string slug);
    Task<(List<Article> Items, int TotalCount)> GetPagedArticles(int page, int pageSize);
    Task<(List<Article> Items, int TotalCount)> GetPagedReviews(int page, int pageSize);
    Task<List<Article>> GetAllArticles();
    Task<List<Article>> GetAllSummary();
    Task Create(Article article);
    Task Update(Article article);
    Task Delete(Guid id);
    Task<Article?> GetByIdWithAll(Guid id);
    Task<List<Article>> GetRecentNews(int count);
}