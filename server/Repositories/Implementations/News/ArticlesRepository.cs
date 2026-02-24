using Context;
using Entities.Models.News;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.News;

namespace Repositories.Implementations.News;

public class ArticlesRepository(EfContext context) : IArticlesRepository
{
    private readonly DbSet<Article> _articles = context.Articles;
    
    public async Task<Article?> GetArticleById(Guid id) => await _articles
        .Include(article => article.Author)
        .Include(article => article.GrandPrix)
        .FirstOrDefaultAsync(article => article.Id == id);
    
    public async Task<Article?> GetArticleBySlug(string slug) => await _articles
        .Include(article => article.Author)
        .Include(article => article.GrandPrix)
        .FirstOrDefaultAsync(article => article.Slug == slug);
    
    public async Task<List<Article>> GetAllArticles() => await _articles
        .Where(article => article.IsSummary != true)
        .ToListAsync();
    
    public async Task<List<Article>> GetAllSummary() => await _articles
        .Where(article => article.IsSummary == true)
        .ToListAsync();
    
    public async Task Create(Article article)
    {
        await _articles.AddAsync(article);
        await context.SaveChangesAsync();
    }

    public async Task Update(Article article)
    {
        _articles.Update(article);
        await context.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        var article = GetArticleById(id).Result;
        if(article == null) return;
        
        _articles.Remove(article);
        await context.SaveChangesAsync();
    }

    public async Task<Article?> GetByIdWithAll(Guid id) => await _articles
        .Include(article => article.Author)
        .Include(article => article.GrandPrix)
        .FirstOrDefaultAsync(article => article.Id == id);

    public async Task<List<Article>> GetRecentNews(int count) => await _articles
        .Include(a => a.Author)
        .Where(a => a.IsSummary != true)
        .OrderByDescending(a => a.DatePublished)
        .Take(count)
        .ToListAsync();
}