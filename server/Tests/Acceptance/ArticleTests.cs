using Context;
using DTOs.News;
using Entities.Models;
using Entities.Models.News;
using FluentAssertions;
using Moq;
using Repositories.Implementations;
using Repositories.Implementations.News;
using Repositories.Implementations.RaceTracks;
using Services.Implementations;
using Services.Interfaces;
using Services.Interfaces.images;
using Xunit;

namespace Tests.Acceptance;

public class ArticleTests
{
    private readonly EfContext _context;
    private readonly IArticleService _service;
    
    public ArticleTests(EfContext context)
    {
        var mockImageService = new Mock<IArticleImageService>();
        var userImageService = new Mock<IUserImageService>();
        
        mockImageService
            .Setup(s => s.GetImageUrl(It.IsAny<string>(), It.IsAny<string>()))
            .Returns((string slug, string imageName) => $"/uploads/images/articles/{slug}/{imageName}");
        
        _context = context;
        _service = new ArticleService(
            new ArticlesRepository(_context),
            new AuthRepository(_context),
            new GrandsPrixRepository(_context),
            mockImageService.Object,
            userImageService.Object
        );
    }

    public ArticleTests(IArticleService service, EfContext context)
    {
        _service = service;
        _context = context;
    }

    [Fact]
    public async Task US_03_AC_01_02_GetArticleBySlug_ShouldReturnCorrectArticleContentAndAuthor()
    {
        var context = InMemoryDbFactory.CreateContext();
        var mockImageService = new Mock<IArticleImageService>();
        var userImageService = new Mock<IUserImageService>();
        mockImageService
            .Setup(s => s.GetImageUrl(It.IsAny<string>(), It.IsAny<string>()))
            .Returns((string slug, string imageName) => $"/uploads/images/articles/{slug}/{imageName}");
        
        var service = new ArticleService(
            new ArticlesRepository(context),
            new AuthRepository(context),
            new GrandsPrixRepository(context),
            mockImageService.Object,
            userImageService.Object
        );

        const string testSlug = "hungaroring-news-2024";

        var author = new User
        {
            Id = Guid.NewGuid(),
            Username = "Author Alex",
            FullName = "Csiger Alex",
            Email = "authoralex@gmail.com",
            PasswordHash = "Password2424!",
            Role = "author",
            Created = DateTime.UtcNow
        };

        var article = new Article
        {
            Id = Guid.NewGuid(),
            Title = "F1 at Hungaroring",
            Slug = testSlug,
            Tag = "F1",
            Lead = "The traveling circus arrives at Hungaroring in June",
            FirstSection = "This is the start of the full content...",
            LastSection = "This is the final section of the content",
            IsSummary = false,
            Author = author
        };

        await context.Users.AddAsync(author);
        await context.Articles.AddAsync(article);
        await context.SaveChangesAsync();

        var result = await service.GetArticleBySlug(testSlug);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Title.Should().Be("F1 at Hungaroring");
        result.Value.Slug.Should().Be(testSlug);
        result.Value.Lead.Should().Be("The traveling circus arrives at Hungaroring in June");
        result.Value.FirstSection.Should().NotBeNullOrEmpty();
        result.Value.LastSection.Should().NotBeNullOrEmpty();
        result.Value.IsReview.Should().BeFalse();
        result.Value.AuthorName.Should().Be("Author Alex");
    }
 
    [Fact]
    public async Task US_02_AC_02_GetRecentArticles_ShouldReturnArticles_SortedByDateDescending()
    {
        var context = InMemoryDbFactory.CreateContext();
        var mockImageService = new Mock<IArticleImageService>();
        var userImageService = new Mock<IUserImageService>();
        mockImageService
            .Setup(s => s.GetImageUrl(It.IsAny<string>(), It.IsAny<string>()))
            .Returns((string slug, string imageName) => $"/uploads/images/articles/{slug}/{imageName}");

        var service = new ArticleService(
            new ArticlesRepository(context),
            new AuthRepository(context),
            new GrandsPrixRepository(context),
            mockImageService.Object,
            userImageService.Object
        );

        var now = DateTime.UtcNow;

        var articles = new List<Article>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Old",
                DatePublished = now.AddDays(-5),
                IsSummary = false,
                Slug = "old",
                Lead = "lead",
                Tag = "F1",
                FirstSection = "First",
                LastSection = "Last"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Latest",
                DatePublished = now,
                IsSummary = false,
                Tag = "F1",
                Slug = "latest",
                Lead = "latest lead",
                FirstSection = "First",
                LastSection = "Last"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Yesterday",
                DatePublished = now.AddDays(-1),
                IsSummary = false,
                Slug = "yesterday",     
                Tag = "F1",
                Lead = "lead",
                FirstSection = "first",
                LastSection = "last"
            }
        };

        await context.Articles.AddRangeAsync(articles);
        await context.SaveChangesAsync();

        var result = await service.GetRecentArticles(3);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);
        result.Value![0].Title.Should().Be("Latest");
        result.Value[1].Title.Should().Be("Yesterday");
        result.Value[2].Title.Should().Be("Old");
        result.Value.Select(a => a.DatePublished)
            .Should().BeInDescendingOrder();
    }
    
    [Fact]
    public async Task GetArticleBySlug_ShouldReturnArticle()
    {
        var author = CreateAuthor();
        var article = CreateArticle(author);

        await _context.Users.AddAsync(author);
        await _context.Articles.AddAsync(article);
        await _context.SaveChangesAsync();

        var result = await _service.GetArticleBySlug(article.Slug);

        result.IsSuccess.Should().BeTrue();
        result.Value!.AuthorName.Should().Be(author.Username);
    }

    [Fact]
    public async Task GetRecentArticles_ShouldReturnSorted()
    {
        await _context.Articles.AddRangeAsync(
            CreateArticle(null, DateTime.UtcNow.AddDays(-1)),
            CreateArticle(null, DateTime.UtcNow)
        );
        await _context.SaveChangesAsync();

        var result = await _service.GetRecentArticles(2);

        result.IsSuccess.Should().BeTrue();
        result.Value![0].DatePublished.Should().BeAfter(result.Value![1].DatePublished);
    }
    
    [Fact]
    public async Task UpdateArticle_ShouldFail_WhenArticleNotFound()
    {
        var dto = new ArticleUpdateDto(Guid.NewGuid(), null, "Title", "Slug", false, "Lead", "First", "Last", null);

        var result = await _service.UpdateArticle(dto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Article not found");
    }

    [Fact]
    public async Task UpdateArticle_ShouldSucceed_And_UpdateFields()
    {
        var article = CreateArticle();
        await _context.Articles.AddAsync(article);
        await _context.SaveChangesAsync();

        var updateDto = new ArticleUpdateDto(
            article.Id, null, "Updated Title", "Slug", true, "updated lead", "Updated First", "Updated Last",
            new SummaryDto("Sec", "Third", "Fourth")
        );

        var result = await _service.UpdateArticle(updateDto);

        result.IsSuccess.Should().BeTrue();
        var updated = await _context.Articles.FindAsync(article.Id);
        updated?.Title.Should().Be("Updated Title");
        updated?.IsSummary.Should().BeTrue();
        updated?.SecondSection.Should().Be("Sec");
    }

    [Fact]
    public async Task CreateArticle_ShouldFail_WhenAuthorNotFound()
    {
        var dto = new ArticleCreateDto(Guid.NewGuid(), Guid.NewGuid(), "T", "S", "F1",false, "L", "F", "L", null);

        var result = await _service.CreateArticle(dto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Author not found");
    }
    
    [Fact]
    public async Task DeleteArticle_ShouldSucceed()
    {
        var article = CreateArticle();
        await _context.Articles.AddAsync(article);
        await _context.SaveChangesAsync();

        var result = await _service.DeleteArticle(article.Id);

        result.IsSuccess.Should().BeTrue();
        _context.Articles.Should().BeEmpty();
    }

    private static User CreateAuthor() => new()
    {
        Id = Guid.NewGuid(),
        Username = "Author",
        FullName = "Petofi Sandor",
        Email = "author@test.com",
        PasswordHash = "pwd",
        Role = "author"
    };

    private static Article CreateArticle(User? author = null, DateTime? published = null) => new()
    {
        Id = Guid.NewGuid(),
        Title = "Article",
        Slug = Guid.NewGuid().ToString(),
        Lead = "Lead",
        Tag = "F1",
        FirstSection = "First",
        LastSection = "Last",
        Author = author,
        AuthorId = author?.Id ?? Guid.Empty,
        DatePublished = published ?? DateTime.UtcNow
    };
}