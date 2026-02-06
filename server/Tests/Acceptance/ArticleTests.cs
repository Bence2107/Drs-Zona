using Context;
using DTOs.News;
using Entities.Models;
using Entities.Models.News;
using FluentAssertions;
using Repositories.Implementations;
using Repositories.Implementations.News;
using Repositories.Implementations.RaceTracks;
using Services.Implementations;
using Services.Implementations.image;
using Services.Interfaces;
using Xunit;

namespace Tests.Acceptance;

public class ArticleTests
{
    private readonly EfContext _context;
    private readonly IArticleService _service;

    /*
    public ArticleTests()
    {
        _context = InMemoryDbFactory.CreateContext();

        _service = new ArticleService(
            new ArticlesRepository(_context),
            new UsersRepository(_context),
            new GrandsPrixRepository(_context),
        );
    }
    
    [Fact]
    public void US_03_AC_01_02_GetArticleBySlug_ShouldReturnCorrectArticleContentAndAuthor()
    {
        var context = InMemoryDbFactory.CreateContext();

        var service = new ArticleService(
            new ArticlesRepository(context),
            new UsersRepository(context),
            new GrandsPrixRepository(context)
        );

        const string testSlug = "hungaroring-news-2024";

        var author = new User
        {
            Id = Guid.NewGuid(),
            Username = "Author Alex",
            Email = "authoralex@gmail.com",
            Password = "Password2424!",
            Role = "author",
            Created = DateTime.UtcNow
        };

        var article = new Article
        {
            Id = Guid.NewGuid(),
            Title = "F1 at Hungaroring",
            Slug = testSlug,
            Lead = "The traveling circus arrives at Hungaroring in June",
            FirstSection = "This is the start of the full content...",
            LastSection = "This is the final section of the content",
            IsSummary = false,
            Author = author
        };

        context.Users.Add(author);
        context.Articles.Add(article);
        context.SaveChanges();

        var result = service.GetArticleBySlug(testSlug);

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
    public void US_02_AC_02_GetRecentArticles_ShouldReturnArticles_SortedByDateDescending()
    {
        var context = InMemoryDbFactory.CreateContext();

        var service = new ArticleService(
            new ArticlesRepository(context),
            new UsersRepository(context),
            new GrandsPrixRepository(context)
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
                FirstSection = "First",
                LastSection = "Last"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Latest",
                DatePublished = now,
                IsSummary = false,
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
                Lead = "lead",
                FirstSection = "first",
                LastSection = "last"
            }
        };

        context.Articles.AddRange(articles);
        context.SaveChanges();

        var result = service.GetRecentArticles(3);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);
        result.Value![0].Title.Should().Be("Latest");
        result.Value[1].Title.Should().Be("Yesterday");
        result.Value[2].Title.Should().Be("Old");
        result.Value.Select(a => a.DatePublished)
            .Should().BeInDescendingOrder();
    }
    
       */
    [Fact]
    public void GetArticleBySlug_ShouldReturnArticle()
    {
        var author = CreateAuthor();
        var article = CreateArticle(author);

        _context.Users.Add(author);
        _context.Articles.Add(article);
        _context.SaveChanges();

        var result = _service.GetArticleBySlug(article.Slug);

        result.IsSuccess.Should().BeTrue();
        result.Value!.AuthorName.Should().Be(author.Username);
    }

    [Fact]
    public void GetRecentArticles_ShouldReturnSorted()
    {
        _context.Articles.AddRange(
            CreateArticle(null, DateTime.UtcNow.AddDays(-1)),
            CreateArticle(null, DateTime.UtcNow)
        );
        _context.SaveChanges();

        var result = _service.GetRecentArticles(2);

        result.IsSuccess.Should().BeTrue();
        result.Value![0].DatePublished.Should().BeAfter(result.Value![1].DatePublished);
    }
    
    [Fact]
    public void UpdateArticle_ShouldFail_WhenArticleNotFound()
    {
        // Arrange
        var dto = new ArticleUpdateDto(Guid.NewGuid(), null, "Title", "Slug", false, "Lead","First", "Last", null);

        // Act
        var result = _service.UpdateArticle(dto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Article not found");
    }

    [Fact]
    public void UpdateArticle_ShouldSucceed_And_UpdateFields()
    {
        // Arrange
        var article = CreateArticle();
        _context.Articles.Add(article);
        _context.SaveChanges();

        var updateDto = new ArticleUpdateDto(
            article.Id, null, "Updated Title", "Slug",true,"updated lead","Updated First", "Updated Last", 
            new SummaryDto("Sec", "Third", "Fourth")
        );

        // Act
        var result = _service.UpdateArticle(updateDto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var updated = _context.Articles.Find(article.Id);
        updated?.Title.Should().Be("Updated Title");
        updated?.IsSummary.Should().BeTrue();
        updated?.SecondSection.Should().Be("Sec");
    }

    [Fact]
    public void CreateArticle_ShouldFail_WhenAuthorNotFound()
    {
        var dto = new ArticleCreateDto(Guid.NewGuid(), Guid.NewGuid(), "T", "S", false, "L", "F", "L", null);

        var result = _service.CreateArticle(dto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Author not found");
    }
    
    [Fact]
    public void DeleteArticle_ShouldSucceed()
    {
        var article = CreateArticle();
        _context.Articles.Add(article);
        _context.SaveChanges();

        var result = _service.DeleteArticle(article.Id);

        result.IsSuccess.Should().BeTrue();
        _context.Articles.Should().BeEmpty();
    }

    private static User CreateAuthor() => new()
    {
        Id = Guid.NewGuid(),
        Username = "Author",
        Email = "author@test.com",
        Password = "pwd",
        Role = "author"
    };

    private static Article CreateArticle(User? author = null, DateTime? published = null) => new()
    {
        Id = Guid.NewGuid(),
        Title = "Article",
        Slug = Guid.NewGuid().ToString(),
        Lead = "Lead",
        FirstSection = "First",
        LastSection = "Last",
        Author = author,
        AuthorId = author!.Id,
        DatePublished = published ?? DateTime.UtcNow
    };
}
