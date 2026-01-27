using Entities.Models.News;

namespace Repositories.Interfaces.News;

public interface IArticlesRepository
{
    Article? GetArticleById(Guid id);
    List<Article> GetAllArticles();
    void Create(Article article);
    void Update(Article article);
    void Delete(Guid id);
    Article? GetByIdWithAuthor(Guid id);
    Article? GetByIdWithGrandPrix(Guid id);
    Article? GetByIdWithAll(Guid id);
    List<Article> GetByAuthorId(Guid authorId);
    List<Article> GetByGrandPrixId(Guid grandPrixId);
    List<Article> GetRecent(int count);
    bool CheckIfIdExists(Guid id);
}