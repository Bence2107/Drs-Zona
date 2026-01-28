using Context;
using Entities.Models.News;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.News;

namespace Repositories.Implementations.News;

public class ArticlesRepository(EfContext context) : IArticlesRepository
{
    private readonly DbSet<Article> _articles = context.Articles;
    
    public Article? GetArticleById(Guid id) => _articles
        .Include(article => article.Author)
        .Include(article => article.GrandPrix)
        .FirstOrDefault(article => article.Id == id);
    
    public Article? GetArticleBySlug(string slug) => _articles
        .Include(article => article.Author)
        .Include(article => article.GrandPrix)
        .FirstOrDefault(article => article.Slug == slug);
    
    public List<Article> GetAllArticles() => _articles
        .Where(article => article.IsSummary != true)
        .ToList();
    
    public List<Article> GetAllSummary() => _articles
        .Where(article => article.IsSummary == true)
        .ToList();
    
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

    public void Delete(Guid id)
    {
        var article = GetArticleById(id);
        if(article == null) return;
        
        _articles.Remove(article);
        context.SaveChanges();
    }

    public Article? GetByIdWithAuthor(Guid id) => _articles
            .Include(article => article.Author)
            .FirstOrDefault(article => article.Id == id);

    public Article? GetByIdWithGrandPrix(Guid id) => _articles
        .Include(article => article.GrandPrix)
        .FirstOrDefault(article => article.Id == id);

    public Article? GetByIdWithAll(Guid id) => _articles
        .Include(article => article.Author)
        .Include(article => article.GrandPrix)
        .FirstOrDefault(article => article.Id == id);
    
    public List<Article> GetByAuthorId(Guid authorId)  => _articles
        .Where(article => article.AuthorId == authorId)
        .ToList();

    public List<Article> GetByGrandPrixId(Guid grandPrixId) => _articles
        .Where(article => article.GrandPrixId == grandPrixId)
        .ToList();

    public List<Article> GetRecentNews(int count) => _articles
        .Include(a => a.Author)
        .Where(a => a.IsSummary != true)
        .OrderByDescending(a => a.DatePublished)
        .Take(count)
        .ToList();
    
    public List<Article> GetRecentSummarys(int count) => _articles
        .Include(a => a.Author)
        .Where(a => a.IsSummary == true)
        .OrderByDescending(a => a.DatePublished)
        .Take(count)
        .ToList();
    
    public bool CheckIfIdExists(Guid id)  => _articles.Any(article => article.Id == id);
}