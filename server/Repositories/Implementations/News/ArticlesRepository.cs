using Context;
using Entities.Models.News;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.News;

namespace Repositories.Implementations.News;

public class ArticlesRepository(EfContext context) : IArticlesRepository
{
    private readonly DbSet<Article> _articles = context.Articles;
    
    public Article? GetArticleById(int id) => _articles.FirstOrDefault(article => article.Id == id);
    
    public List<Article> GetAllArticles() => _articles.ToList();
    
    public void Create(Article article)
    {
        _articles.Add(article);
        context.SaveChanges();
    }

    public void Update(Article article)
    {
        _articles.Update(article);
        context.SaveChanges();
    }

    public void Delete(int id)
    {
        var article = GetArticleById(id);
        if(article == null) return;
        
        _articles.Remove(article);
        context.SaveChanges();
    }

    public Article? GetByIdWithAuthor(int id) => _articles
            .Include(article => article.Author)
            .FirstOrDefault(article => article.Id == id);

    public Article? GetByIdWithGrandPrix(int id) => _articles
        .Include(article => article.GrandPrix)
        .FirstOrDefault(article => article.Id == id);

    public Article? GetByIdWithAll(int id) => _articles
        .Include(article => article.Author)
        .Include(article => article.GrandPrix)
        .FirstOrDefault(article => article.Id == id);


    public List<Article> GetByAuthorId(int authorId)  => _articles
        .Where(article => article.AuthorId == authorId)
        .ToList();

    public List<Article> GetByGrandPrixId(int grandPrixId) => _articles
        .Where(article => article.GrandPrixId == grandPrixId)
        .ToList();

    public List<Article> GetRecent(int count) => _articles
        .OrderByDescending(a => a.DatePublished)
        .Take(count)
        .ToList();
    
    public bool CheckIfIdExists(int id)  => _articles.Any(article => article.Id == id);
}