using Context;
using Entities.Models.News;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.News;

namespace Repositories.Implementations.News;

public class ArticlesRepository(EfContext context) : IArticlesRepository 
{
    private readonly DbSet<Article> _articles = context.Articles;
    
    public async Task<Article?> GetById(Guid id) => await _articles
        .Include(article => article.Author)
        .FirstOrDefaultAsync(article => article.Id == id);
    
    public async Task<Article?> GetByIdWithAll(Guid id) => await _articles
        .Include(article => article.Author)
        .FirstOrDefaultAsync(article => article.Id == id);
    
    public async Task<List<Article>> GetRecentNews(int count, string? tag = null)
    {
        var articles = await _articles
            .Include(a => a.Author)
            .Where(a => a.IsSummary != true)
            .OrderByDescending(a => a.DatePublished)
            .ToListAsync();
        
        if (!string.IsNullOrEmpty(tag))
        {
            articles = articles.Where(t => t.Tag == tag).ToList();
        }
        
        return articles.Take(count).ToList();
    } 
    
    public async Task<Article?> GetArticleBySlug(string slug) => await _articles
        .Include(article => article.Author)
        .FirstOrDefaultAsync(article => article.Slug == slug);

    public async Task<(List<Article> Items, int TotalCount)> GetPagedArticles(int page, int pageSize, string? tag)
    {
        var query = _articles.Where(a => a.IsSummary != true);

        if (!string.IsNullOrEmpty(tag))
        {
            query = query.Where(a => a.Tag == tag);
        }

        var totalCount = await query.CountAsync();
        
        var items = await query
            .OrderByDescending(a => a.DatePublished)
            .Skip((page) * pageSize) 
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
    
    public async Task<(List<Article> Items, int TotalCount)> GetPagedReviews(int page, int pageSize, string? tag)
    {
        var query = _articles.Where(a => a.IsSummary == true);

        if (!string.IsNullOrEmpty(tag))
        {
            query = query.Where(a => a.Tag == tag);
        }
        
        var totalCount = await query.CountAsync();
        
        var items = await query
            .OrderByDescending(a => a.DatePublished)
            .Skip((page) * pageSize) 
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
    
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
        var article = GetById(id).Result;
        if(article == null) return;
        
        _articles.Remove(article);
        await context.SaveChangesAsync();
    }
}