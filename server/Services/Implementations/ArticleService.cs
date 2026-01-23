using DTOs.News;
using Entities.Models.News;
using Repositories.Interfaces;
using Repositories.Interfaces.News;
using Repositories.Interfaces.RaceTracks;
using Services.Interfaces;

namespace Services.Implementations;

public class ArticleService(
    IArticlesRepository articleRepo,
    IUsersRepository userRepo,
    IGrandsPrixRepository gpRepo
) : IArticleService
{
    public ResponseResult<ArticleDetailDto> GetArticleById(int id)
    {
        var article = articleRepo.GetByIdWithAll(id);
        if (article == null) return ResponseResult<ArticleDetailDto>.Failure("Article not found.");
        
        return ResponseResult<ArticleDetailDto>.Success(new ArticleDetailDto(
            Id: article.Id,
            Title: article.Title,
            Lead: article.Lead,
            Content: article.Content,
            AuthorId: article.AuthorId,
            AuthorName: article.Author!.Username,
            GrandPrixId: article.GrandPrixId,
            GrandPrixName: article.GrandPrix!.Name,
            DatePublished: article.DatePublished,
            DateUpdated: article.DateUpdated
        ));

    }

    public ResponseResult<List<ArticleListDto>> ListArticles()
    {
        var articles = articleRepo.GetAllArticles().Select(a => new ArticleListDto(
            Id: a.Id,
            Title: a.Title,
            Lead: a.Lead,
            DatePublished: a.DatePublished
        )).ToList();
        
        return ResponseResult<List<ArticleListDto>>.Success(articles);
    }

    public ResponseResult<List<ArticleRecentDto>> GetRecentArticles(int count)
    {
        var articles = articleRepo.GetRecent(count).Select(a => new ArticleRecentDto(
            Id: a.Id,
            Title: a.Title,
            Lead: a.Lead,
            AuthorName: a.Author!.Username,
            DatePublished: a.DatePublished
        )).ToList();
        
        return ResponseResult<List<ArticleRecentDto>>.Success(articles);
    }

    public ResponseResult<bool> CreateArticle(ArticleCreateDto dto, int authorId)
    {
        if (!userRepo.CheckIfIdExists(authorId))
            return ResponseResult<bool>.Failure("AuthorId", "Author not found");

        if (dto.GrandPrixId.HasValue && !gpRepo.CheckIfIdExists(dto.GrandPrixId.Value))
            return ResponseResult<bool>.Failure("GrandPrixId", "Grand Prix not found");

        var article = new Article
        {
            Title = dto.Title,
            Lead = dto.Lead,
            Content = dto.Content,
            AuthorId = authorId,
            GrandPrixId = dto.GrandPrixId,
            DatePublished = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow
        };

        articleRepo.Create(article);
        return ResponseResult<bool>.Success(true);
    }

    public ResponseResult<bool> UpdateArticle(ArticleUpdateDto dto)
    {
        var article = articleRepo.GetArticleById(dto.Id);
        if (article == null) return ResponseResult<bool>.Failure("Article not found");

        if (dto.GrandPrixId.HasValue && !gpRepo.CheckIfIdExists(dto.GrandPrixId.Value))
            return ResponseResult<bool>.Failure("GrandPrixId", "Grand Prix not found");

        article.GrandPrixId = dto.GrandPrixId;
        article.Title = dto.Title;
        article.Lead = dto.Lead;
        article.Content = dto.Content;
        article.DateUpdated = DateTime.UtcNow;


        articleRepo.Update(article);
        return ResponseResult<bool>.Success(true);
    }

    public ResponseResult<bool> DeleteArticle(int id)
    {
        var article = articleRepo.GetArticleById(id);
        if (article == null)
        {
            return ResponseResult<bool>.Failure("Article not found");
        }

        articleRepo.Delete(article.Id);
        return ResponseResult<bool>.Success(true);
    }
}