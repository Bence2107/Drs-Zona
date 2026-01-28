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
    public ResponseResult<ArticleDetailDto> GetArticleById(Guid id)
    {
        var article = articleRepo.GetByIdWithAll(id);
        if (article == null) return ResponseResult<ArticleDetailDto>.Failure("Article not found.");

        var s2 = article.SecondSection;
        var s3 = article.ThirdSection;
        var s4 = article.FourthSection;
        
        var middleSections = new List<string?> { s2, s3, s4 }
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Cast<string>()
            .ToList();
        
        return ResponseResult<ArticleDetailDto>.Success( new ArticleDetailDto(
            Id: article.Id,
            Title: article.Title,
            Lead: article.Lead,
            IsReview: article.IsSummary,
            FirstSection: article.FirstSection,
            LastSection: article.LastSection,
            MiddleSections: middleSections,
            AuthorId: article.AuthorId,
            AuthorName: article.Author!.Username,
            GrandPrixId: article.GrandPrixId,
            GrandPrixName: article.GrandPrix!.Name,
            DatePublished: article.DatePublished,
            DateUpdated: article.DateUpdated
        ));
    }
    public ResponseResult<ArticleDetailDto> GetArticleBySlug(string slug)
    {
        var article = articleRepo.GetArticleBySlug(slug);
        if (article == null) return ResponseResult<ArticleDetailDto>.Failure("Article not found.");

        var s2 = article.SecondSection;
        var s3 = article.ThirdSection;
        var s4 = article.FourthSection;
        
        var middleSections = new List<string?> { s2, s3, s4 }
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Cast<string>()
            .ToList();
        
        return ResponseResult<ArticleDetailDto>.Success( new ArticleDetailDto(
            Id: article.Id,
            Title: article.Title,
            Lead: article.Lead,
            IsReview: article.IsSummary,
            FirstSection: article.FirstSection,
            LastSection: article.LastSection,
            MiddleSections: middleSections,
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
            Slug: a.Slug,
            DatePublished: a.DatePublished
        )).ToList();
        
        return ResponseResult<List<ArticleListDto>>.Success(articles);
    }
    public ResponseResult<List<ArticleListDto>> ListAllSummary()
    {
        var articles = articleRepo.GetAllSummary().Select(a => new ArticleListDto(
            Id: a.Id,
            Title: a.Title,
            Lead: a.Lead,
            Slug: a.Slug,
            DatePublished: a.DatePublished
        )).ToList();
        
        return ResponseResult<List<ArticleListDto>>.Success(articles);
    }

    public ResponseResult<List<ArticleListDto>> GetRecentArticles(int count)
    {
        var articles = articleRepo.GetRecentNews(count).Select(a => new ArticleListDto(
            Id: a.Id,
            Title: a.Title,
            Lead: a.Lead,
            Slug: a.Slug,
            DatePublished: a.DatePublished
        )).ToList();
        
        return ResponseResult<List<ArticleListDto>>.Success(articles);
    }

    public ResponseResult<bool> CreateArticle(ArticleCreateDto dto)
    {
        if (!userRepo.CheckIfIdExists(dto.AuthorId))
            return ResponseResult<bool>.Failure("AuthorId", "Author not found");

        if (dto.GrandPrixId != null && !gpRepo.CheckIfIdExists(dto.GrandPrixId.Value))
            return ResponseResult<bool>.Failure("GrandPrixId", "Grand Prix not found");
        
        var article = new Article
        {
            Title = dto.Title,
            Lead = dto.Lead,
            Slug = dto.Slug,
            IsSummary = dto.IsReview,
            FirstSection = dto.FirstSection,
            LastSection = dto.LastSection,
            AuthorId = dto.AuthorId,
            GrandPrixId = dto.GrandPrixId,
            DatePublished = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow
        };

        if (dto.Summary != null)
        {
            article.SecondSection = dto.Summary.SecondSection;
            article.ThirdSection = dto.Summary.ThirdSection;
            article.FourthSection = dto.Summary.FourthSection;
        }

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
        article.IsSummary = dto.IsReview;
        article.FirstSection = dto.FirstSection;
        article.LastSection = dto.LastSection;
        article.DateUpdated = DateTime.UtcNow;

        if (dto.Summary != null)
        {
            article.SecondSection = dto.Summary.SecondSection;
            article.ThirdSection = dto.Summary.ThirdSection;
            article.FourthSection = dto.Summary.FourthSection;
        }

        articleRepo.Update(article);
        return ResponseResult<bool>.Success(true);
    }

    public ResponseResult<bool> DeleteArticle(Guid id)
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