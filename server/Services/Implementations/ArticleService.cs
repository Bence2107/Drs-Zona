using DTOs.News;
using Entities.Models.News;
using Repositories.Interfaces;
using Repositories.Interfaces.News;
using Repositories.Interfaces.RaceTracks;
using Services.Interfaces;
using Services.Interfaces.images;
using Services.Types;

namespace Services.Implementations;

public class ArticleService(
    IArticlesRepository articleRepo,
    IAuthRepository userRepo,
    IGrandsPrixRepository gpRepo,
    IArticleImageService articleImageService, 
    IUserImageService userImageService
) : IArticleService
{
    public async Task<ResponseResult<ArticleDetailDto>> GetArticleById(Guid id)
    {
        var article = await articleRepo.GetByIdWithAll(id);
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
            Slug: article.Slug,
            Tag: article.Tag,
            IsReview: article.IsSummary,
            FirstSection: article.FirstSection,
            LastSection: article.LastSection,
            MiddleSections: middleSections,
            AuthorId: article.AuthorId,
            AuthorName: article.Author!.FullName,
            DatePublished: article.DatePublished,
            DateUpdated: article.DateUpdated,
            PrimaryImageUrl: articleImageService.GetImageUrl(article.Slug, "primary.jpg"),
            SecondaryImageUrl: article.IsSummary ? articleImageService.GetImageUrl(article.Slug, "secondary.jpg") : null,
            ThirdImageUrl: article.IsSummary ? articleImageService.GetImageUrl(article.Slug, "third.jpg") : null,
            LastImageUrl: articleImageService.GetImageUrl(article.Slug, "last.jpg"),
            AuthorImageUrl: await userImageService.GetAvatarUrl(article.AuthorId)
        ));
    }
    
    public async Task<ResponseResult<ArticleDetailDto>> GetArticleBySlug(string slug)
    {
        var article = await articleRepo.GetArticleBySlug(slug);
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
            Slug: article.Slug,
            Tag: article.Tag,
            IsReview: article.IsSummary,
            FirstSection: article.FirstSection,
            LastSection: article.LastSection,
            MiddleSections: middleSections,
            AuthorId: article.AuthorId,
            AuthorName: article.Author!.FullName,
            DatePublished: article.DatePublished,
            DateUpdated: article.DateUpdated,
            PrimaryImageUrl: articleImageService.GetImageUrl(article.Slug, "primary.jpg"),
            SecondaryImageUrl: article.IsSummary ? articleImageService.GetImageUrl(article.Slug, "secondary.jpg") : null,
            ThirdImageUrl: article.IsSummary ? articleImageService.GetImageUrl(article.Slug, "third.jpg") : null,
            LastImageUrl: articleImageService.GetImageUrl(article.Slug, "last.jpg"),
            AuthorImageUrl: await userImageService.GetAvatarUrl(article.AuthorId)
        ));
    }

    public async Task<ResponseResult<PagedResult<ArticleListDto>>> ListArticles(int page, int pageSize, string? tag = null)
    {
        var (articles, totalCount) = await articleRepo.GetPagedArticles(page, pageSize, tag);      
        
        var dtoS = articles.Select(a => new ArticleListDto(
                Id: a.Id,
                Title: a.Title,
                Lead: a.Lead,
                Tag: a.Tag,
                IsReview: a.IsSummary,
                Slug: a.Slug,
                DatePublished: a.DatePublished,
                PrimaryImageUrl: articleImageService.GetImageUrl(a.Slug, "primary.jpg")
            ))
            .OrderByDescending(a => a.DatePublished)
            .ToList();
        
        var result = new PagedResult<ArticleListDto>
        {
            Items = dtoS,
            TotalCount = totalCount,
            PageSize = pageSize,
            CurrentPage = page
        };
        
        return ResponseResult<PagedResult<ArticleListDto>>.Success(result);
    }
    
    public async Task<ResponseResult<PagedResult<ArticleListDto>>> ListAllSummary(int page, int pageSize, string? tag = null)
    {
        var (articles, totalCount) = await articleRepo.GetPagedReviews(page, pageSize, tag);      
        
        var dtoS = articles.Select(a => new ArticleListDto(
                Id: a.Id,
                Title: a.Title,
                Lead: a.Lead,
                Tag: a.Tag,
                IsReview: a.IsSummary,
                Slug: a.Slug,
                DatePublished: a.DatePublished,
                PrimaryImageUrl: articleImageService.GetImageUrl(a.Slug, "primary.jpg")
            ))
            .OrderByDescending(a => a.DatePublished)
            .ToList();
        
        var result = new PagedResult<ArticleListDto>
        {
            Items = dtoS,
            TotalCount = totalCount,
            PageSize = pageSize,
            CurrentPage = page
        };
        
        return ResponseResult<PagedResult<ArticleListDto>>.Success(result);
    }

    public async Task<ResponseResult<List<ArticleListDto>>> GetRecentArticles(int count, string? tag = null)
    {
        var articles = await articleRepo.GetRecentNews(count, tag);
        
        var dtoS = articles.Select(a => new ArticleListDto(
                Id: a.Id,
                Title: a.Title,
                Lead: a.Lead,
                Tag: a.Tag,
                IsReview: a.IsSummary,
                Slug: a.Slug,
                DatePublished: a.DatePublished,
                PrimaryImageUrl: articleImageService.GetImageUrl(a.Slug, "primary.jpg")
            ))
            .OrderByDescending(a => a.DatePublished)
            .ToList();
        
        return ResponseResult<List<ArticleListDto>>.Success(dtoS);
    }

    public async Task<ResponseResult<bool>> CreateArticle(ArticleCreateDto dto)
    {
        if (!await userRepo.CheckIfIdExists(dto.AuthorId))
            return ResponseResult<bool>.Failure("AuthorId", "Author not found");

        if (dto.GrandPrixId != null && !await gpRepo.CheckIfIdExists(dto.GrandPrixId.Value))
            return ResponseResult<bool>.Failure("GrandPrixId", "Grand Prix not found");
        
        var article = new Article
        {
            Title = dto.Title,
            Lead = dto.Lead,
            Tag = dto.Tag,
            Slug = dto.Slug,
            IsSummary = dto.IsReview,
            FirstSection = dto.FirstSection,
            LastSection = dto.LastSection,
            AuthorId = dto.AuthorId,
            DatePublished = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow
        };

        if (dto.Summary != null)
        {
            article.SecondSection = dto.Summary.SecondSection;
            article.ThirdSection = dto.Summary.ThirdSection;
            article.FourthSection = dto.Summary.FourthSection;
        }

        await articleRepo.Create(article);
        return ResponseResult<bool>.Success(true);
    }

    public async Task<ResponseResult<bool>> UpdateArticle(ArticleUpdateDto dto)
    {
        var article = await articleRepo.GetArticleById(dto.Id);
        if (article == null) return ResponseResult<bool>.Failure("Article not found");

        if (dto.GrandPrixId.HasValue && !await gpRepo.CheckIfIdExists(dto.GrandPrixId.Value))
            return ResponseResult<bool>.Failure("GrandPrixId", "Grand Prix not found");

        article.Id = dto.Id;
        article.Title = dto.Title;
        article.Lead = dto.Lead;
        article.Slug = dto.Slug;
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

        await articleRepo.Update(article);
        return ResponseResult<bool>.Success(true);
    }

    public async Task<ResponseResult<bool>> DeleteArticle(Guid id)
    {
        var article = await articleRepo.GetArticleById(id);
        if (article == null)
        {
            return ResponseResult<bool>.Failure("Article not found");
        }

        await articleRepo.Delete(article.Id);
        return ResponseResult<bool>.Success(true);
    }
}