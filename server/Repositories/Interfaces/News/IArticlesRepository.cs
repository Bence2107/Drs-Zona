using Entities.Models.News;

namespace Repositories.Interfaces.News;

public interface IArticlesRepository
{
    Article? GetArticleById(Guid id);
    Article? GetArticleBySlug(string slug);
    List<Article> GetAllArticles();
    List<Article> GetAllSummary();
    void Create(Article article);
    void Update(Article article);
    void Delete(Guid id);
    Article? GetByIdWithAuthor(Guid id);
    Article? GetByIdWithGrandPrix(Guid id);
    Article? GetByIdWithAll(Guid id);
    List<Article> GetByAuthorId(Guid authorId);
    List<Article> GetByGrandPrixId(Guid grandPrixId);
    List<Article> GetRecentNews(int count);
    List<Article> GetRecentSummarys(int count);
    bool CheckIfIdExists(Guid id);
}