using DTOs.News;
using Entities.Models;
using Entities.Models.News;
using FluentAssertions;
using Moq;
using Repositories.Interfaces;
using Repositories.Interfaces.News;
using Repositories.Interfaces.RaceTracks;
using Services.Implementations;
using Services.Interfaces.images;
using Xunit;

namespace Tests.Services.Units;

public class ArticleServiceUnitTests
{

    private readonly Mock<IArticlesRepository>   _articleRepo  = new();
    private readonly Mock<IAuthRepository>        _userRepo     = new();
    private readonly Mock<IGrandsPrixRepository>  _gpRepo       = new();
    private readonly Mock<IArticleImageService>   _imageService = new();
    private readonly Mock<IUserImageService>      _userImage    = new();

    private readonly ArticleService _svc;

    public ArticleServiceUnitTests()
    {
        _imageService
            .Setup(s => s.GetImageUrl(It.IsAny<string>(), It.IsAny<string>()))
            .Returns((string slug, string img) => $"/uploads/images/articles/{slug}/{img}");

        _userImage
            .Setup(s => s.GetAvatarUrl(It.IsAny<Guid>()))
            .ReturnsAsync((Guid id) => $"/uploads/avatars/{id}.jpg");

        _svc = new ArticleService(
            _articleRepo.Object,
            _userRepo.Object,
            _gpRepo.Object,
            _imageService.Object,
            _userImage.Object
        );
    }

    // ─────────────────────────────────────────────
    // GetArticleBySlug
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetArticleBySlug_ShouldReturnFailure_WhenArticleNotFound()
    {
        _articleRepo
            .Setup(r => r.GetArticleBySlug("missing-slug"))
            .ReturnsAsync((Article?)null);

        var result = await _svc.GetArticleBySlug("missing-slug");

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Article not found.");
    }

    [Fact]
    public async Task GetArticleBySlug_ShouldReturnArticle_WithCorrectFields()
    {
        var author  = CreateAuthor();
        var article = CreateArticle(author);

        _articleRepo
            .Setup(r => r.GetArticleBySlug(article.Slug))
            .ReturnsAsync(article);

        var result = await _svc.GetArticleBySlug(article.Slug);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Title.Should().Be(article.Title);
        result.Value.Slug.Should().Be(article.Slug);
        result.Value.AuthorName.Should().Be(author.FullName);
        result.Value.IsReview.Should().BeFalse();
    }

    [Fact]
    public async Task GetArticleBySlug_ShouldReturnMiddleSections_WhenSummary()
    {
        var author  = CreateAuthor();
        var article = CreateArticle(author, isSummary: true);
        article.SecondSection = "Second";
        article.ThirdSection  = "Third";
        article.FourthSection = "Fourth";

        _articleRepo
            .Setup(r => r.GetArticleBySlug(article.Slug))
            .ReturnsAsync(article);

        var result = await _svc.GetArticleBySlug(article.Slug);

        result.IsSuccess.Should().BeTrue();
        result.Value!.MiddleSections.Should().HaveCount(3);
        result.Value.MiddleSections[0].Should().Be("Second");
    }

    [Fact]
    public async Task GetArticleBySlug_ShouldReturnEmptyMiddleSections_WhenNotSummary()
    {
        var article = CreateArticle(CreateAuthor(), isSummary: false);

        _articleRepo
            .Setup(r => r.GetArticleBySlug(article.Slug))
            .ReturnsAsync(article);

        var result = await _svc.GetArticleBySlug(article.Slug);

        result.IsSuccess.Should().BeTrue();
        result.Value!.MiddleSections.Should().BeEmpty();
    }

    [Fact]
    public async Task GetArticleBySlug_Summary_ShouldHaveSecondaryImageUrls()
    {
        var article = CreateArticle(CreateAuthor(), slug: "my-slug", isSummary: true);

        _articleRepo
            .Setup(r => r.GetArticleBySlug("my-slug"))
            .ReturnsAsync(article);

        var result = await _svc.GetArticleBySlug("my-slug");

        result.IsSuccess.Should().BeTrue();
        result.Value!.SecondaryImageUrl.Should().Be("/uploads/images/articles/my-slug/secondary.jpg");
        result.Value.ThirdImageUrl.Should().Be("/uploads/images/articles/my-slug/third.jpg");
    }

    [Fact]
    public async Task GetArticleBySlug_NonSummary_ShouldHaveNullSecondaryImageUrls()
    {
        var article = CreateArticle(CreateAuthor(), isSummary: false);

        _articleRepo
            .Setup(r => r.GetArticleBySlug(article.Slug))
            .ReturnsAsync(article);

        var result = await _svc.GetArticleBySlug(article.Slug);

        result.IsSuccess.Should().BeTrue();
        result.Value!.SecondaryImageUrl.Should().BeNull();
        result.Value.ThirdImageUrl.Should().BeNull();
    }

    // ─────────────────────────────────────────────
    // GetArticleById
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetArticleById_ShouldReturnFailure_WhenNotFound()
    {
        var id = Guid.NewGuid();
        _articleRepo
            .Setup(r => r.GetByIdWithAll(id))
            .ReturnsAsync((Article?)null);

        var result = await _svc.GetArticleById(id);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Article not found.");
    }

    [Fact]
    public async Task GetArticleById_ShouldReturnArticle_WhenExists()
    {
        var author  = CreateAuthor();
        var article = CreateArticle(author);

        _articleRepo
            .Setup(r => r.GetByIdWithAll(article.Id))
            .ReturnsAsync(article);

        var result = await _svc.GetArticleById(article.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(article.Id);
        result.Value.AuthorName.Should().Be(author.FullName);
    }

    // ─────────────────────────────────────────────
    // GetRecentArticles
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetRecentArticles_ShouldReturnSortedByDateDescending()
    {
        var now = DateTime.UtcNow;
        var articles = new List<Article>
        {
            CreateArticle(published: now.AddDays(-2), title: "Old"),
            CreateArticle(published: now,             title: "Latest"),
            CreateArticle(published: now.AddDays(-1), title: "Yesterday"),
        };

        _articleRepo
            .Setup(r => r.GetRecentNews(3, null))
            .ReturnsAsync(articles);

        var result = await _svc.GetRecentArticles(3);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Select(a => a.DatePublished).Should().BeInDescendingOrder();
    }

    [Fact]
    public async Task GetRecentArticles_ShouldReturnEmptyList_WhenNoArticles()
    {
        _articleRepo
            .Setup(r => r.GetRecentNews(5, null))
            .ReturnsAsync(new List<Article>());

        var result = await _svc.GetRecentArticles(5);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    // ─────────────────────────────────────────────
    // Create
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Create_ShouldFail_WhenAuthorNotFound()
    {
        var authorId = Guid.NewGuid();

        _userRepo
            .Setup(r => r.CheckIfIdExists(authorId))
            .ReturnsAsync(false);

        var dto = new ArticleCreateDto(
            null, authorId, "Title", "slug", "F1", false,
            "Lead", "First", "Last", null
        );

        var result = await _svc.Create(dto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Author not found");
    }

    [Fact]
    public async Task Create_ShouldFail_WhenGrandPrixNotFound()
    {
        var authorId = Guid.NewGuid();
        var gpId     = Guid.NewGuid();

        _userRepo.Setup(r => r.CheckIfIdExists(authorId)).ReturnsAsync(true);
        _gpRepo.Setup(r => r.CheckIfIdExists(gpId)).ReturnsAsync(false);

        var dto = new ArticleCreateDto(
            gpId, authorId, "Title", "slug", "F1", false,
            "Lead", "First", "Last", null
        );

        var result = await _svc.Create(dto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Grand Prix not found");
    }

    [Fact]
    public async Task Create_ShouldSucceed_WhenAuthorExists()
    {
        var authorId = Guid.NewGuid();

        _userRepo.Setup(r => r.CheckIfIdExists(authorId)).ReturnsAsync(true);
        _articleRepo.Setup(r => r.Create(It.IsAny<Article>())).Returns(Task.CompletedTask);

        var dto = new ArticleCreateDto(
            null, authorId, "Title", "slug", "F1", false,
            "Lead", "First", "Last", null
        );

        var result = await _svc.Create(dto);

        result.IsSuccess.Should().BeTrue();
        _articleRepo.Verify(r => r.Create(It.IsAny<Article>()), Times.Once);
    }

    [Fact]
    public async Task Create_ShouldPassSummarySections_WhenProvided()
    {
        var authorId = Guid.NewGuid();
        Article? saved = null;

        _userRepo.Setup(r => r.CheckIfIdExists(authorId)).ReturnsAsync(true);
        _articleRepo
            .Setup(r => r.Create(It.IsAny<Article>()))
            .Callback<Article>(a => saved = a)
            .Returns(Task.CompletedTask);

        var dto = new ArticleCreateDto(
            null, authorId, "Title", "slug", "F1", true,
            "Lead", "First", "Last",
            new SummaryDto("Second", "Third", "Fourth")
        );

        await _svc.Create(dto);

        saved.Should().NotBeNull();
        saved!.SecondSection.Should().Be("Second");
        saved.ThirdSection.Should().Be("Third");
        saved.FourthSection.Should().Be("Fourth");
    }

    // ─────────────────────────────────────────────
    // Update
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Update_ShouldFail_WhenArticleNotFound()
    {
        _articleRepo
            .Setup(r => r.GetById(It.IsAny<Guid>()))
            .ReturnsAsync((Article?)null);

        var dto = new ArticleUpdateDto(
            Guid.NewGuid(), null, "Title", "slug", false, "Lead", "First", "Last", null
        );

        var result = await _svc.Update(dto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Article not found");
    }

    [Fact]
    public async Task Update_ShouldFail_WhenGrandPrixNotFound()
    {
        var article = CreateArticle();
        var gpId    = Guid.NewGuid();

        _articleRepo.Setup(r => r.GetById(article.Id)).ReturnsAsync(article);
        _gpRepo.Setup(r => r.CheckIfIdExists(gpId)).ReturnsAsync(false);

        var dto = new ArticleUpdateDto(
            article.Id, gpId, "Title", "slug", false, "Lead", "First", "Last", null
        );

        var result = await _svc.Update(dto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Grand Prix not found");
    }

    [Fact]
    public async Task Update_ShouldSucceed_AndCallRepositoryUpdate()
    {
        var article = CreateArticle();

        _articleRepo.Setup(r => r.GetById(article.Id)).ReturnsAsync(article);
        _articleRepo.Setup(r => r.Update(It.IsAny<Article>())).Returns(Task.CompletedTask);

        var dto = new ArticleUpdateDto(
            article.Id, null, "New Title", "new-slug", true,
            "New lead", new string('A', 100), new string('B', 100), null
        );

        var result = await _svc.Update(dto);

        result.IsSuccess.Should().BeTrue();
        _articleRepo.Verify(r => r.Update(It.Is<Article>(a =>
            a.Title == "New Title" &&
            a.Slug  == "new-slug"  &&
            a.IsSummary == true
        )), Times.Once);
    }

    [Fact]
    public async Task Update_ShouldUpdateSummarySections_WhenProvided()
    {
        var article = CreateArticle();
        Article? updated = null;

        _articleRepo.Setup(r => r.GetById(article.Id)).ReturnsAsync(article);
        _articleRepo
            .Setup(r => r.Update(It.IsAny<Article>()))
            .Callback<Article>(a => updated = a)
            .Returns(Task.CompletedTask);

        var dto = new ArticleUpdateDto(
            article.Id, null, "Title", "slug", true, "Lead",
            new string('F', 100), new string('L', 100),
            new SummaryDto("S2", "S3", "S4")
        );

        await _svc.Update(dto);

        updated!.SecondSection.Should().Be("S2");
        updated.ThirdSection.Should().Be("S3");
        updated.FourthSection.Should().Be("S4");
    }

    // ─────────────────────────────────────────────
    // Delete
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Delete_ShouldFail_WhenArticleNotFound()
    {
        var id = Guid.NewGuid();
        _articleRepo.Setup(r => r.GetById(id)).ReturnsAsync((Article?)null);

        var result = await _svc.Delete(id);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Article not found");
    }

    [Fact]
    public async Task Delete_ShouldSucceed_AndCallRepositoryDelete()
    {
        var article = CreateArticle();

        _articleRepo.Setup(r => r.GetById(article.Id)).ReturnsAsync(article);
        _articleRepo.Setup(r => r.Delete(article.Id)).Returns(Task.CompletedTask);

        var result = await _svc.Delete(article.Id);

        result.IsSuccess.Should().BeTrue();
        _articleRepo.Verify(r => r.Delete(article.Id), Times.Once);
    }

    // ─────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────

    private static User CreateAuthor() => new()
    {
        Id           = Guid.NewGuid(),
        Username     = "author_user",
        FullName     = "Test Elek",
        Email        = $"{Guid.NewGuid()}@test.com",
        PasswordHash = "pwd",
        Role         = "author"
    };

    private static Article CreateArticle(
        User?     author    = null,
        string?   slug      = null,
        string?   title     = null,
        bool      isSummary = false,
        DateTime? published = null) => new()
    {
        Id            = Guid.NewGuid(),
        Title         = title     ?? "Test Article",
        Slug          = slug      ?? Guid.NewGuid().ToString(),
        Lead          = "Test lead",
        Tag           = "F1",
        IsSummary     = isSummary,
        FirstSection  = "First",
        LastSection   = "Last",
        Author        = author,
        AuthorId      = author?.Id ?? Guid.NewGuid(),
        DatePublished = published  ?? DateTime.UtcNow,
        DateUpdated   = DateTime.UtcNow
    };
}