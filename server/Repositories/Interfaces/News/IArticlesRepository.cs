using Entities.Models.News;

namespace Repositories.Interfaces.News;

public interface IArticlesRepository 
{
    Task<Article?> GetById(Guid id);
    Task<Article?> GetByIdWithAll(Guid id);
    Task<Article?> GetArticleBySlug(string slug);
    Task<List<Article>> GetRecentNews(int count, string? tag = null);
    Task<(List<Article> Items, int TotalCount)> GetPagedArticles(int page, int pageSize, string? tag = null);
    Task<(List<Article> Items, int TotalCount)> GetPagedReviews(int page, int pageSize, string? tag = null);
    Task Create(Article article);
    Task Update(Article article);
    Task Delete(Guid id);
}