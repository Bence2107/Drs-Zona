using Entities.Models.News;

namespace Repositories.Interfaces.News;

public interface IArticlesRepository
{
    Article? GetArticleById(int id);
    List<Article> GetAllArticles();
    void Create(Article article);
    void Update(Article article);
    void Delete(int id);
    Article? GetByIdWithAuthor(int id);
    Article? GetByIdWithGrandPrix(int id);
    Article? GetByIdWithAll(int id);
    List<Article> GetByAuthorId(int authorId);
    List<Article> GetByGrandPrixId(int grandPrixId);
    List<Article> GetRecent(int count);
    bool CheckIfIdExists(int id);
}