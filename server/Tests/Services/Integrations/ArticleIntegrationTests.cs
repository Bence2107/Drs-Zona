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

namespace Tests.Services.Integrations;

public class ArticleServiceIntegrationTests
{
    private static (EfContext context, IArticleService service) BuildSut()
    {
        var context = InMemoryDbFactory.CreateContext();

        var mockImageService = new Mock<IArticleImageService>();
        var userImageService = new Mock<IUserImageService>();

        mockImageService
            .Setup(s => s.GetImageUrl(It.IsAny<string>(), It.IsAny<string>()))
            .Returns((string slug, string imageName) => $"/uploads/images/articles/{slug}/{imageName}");

        userImageService
            .Setup(s => s.GetAvatarUrl(It.IsAny<Guid>()))
            .ReturnsAsync((Guid id) => $"/uploads/avatars/{id}.jpg");

        var service = new ArticleService(
            new ArticlesRepository(context),
            new AuthRepository(context),
            new GrandsPrixRepository(context),
            mockImageService.Object,
            userImageService.Object
        );

        return (context, service);
    }
    
    private static async Task<(User author, Article article)> SeedArticleWithAuthor(
        EfContext ctx,
        bool isSummary = false,
        string? slug = null,
        string? title = null,
        string? lead = null,
        DateTime? published = null)
    {
        var author  = CreateAuthor();
        var article = CreateArticle(author: author, isSummary: isSummary,
            slug: slug, title: title, lead: lead, published: published);

        await ctx.Users.AddAsync(author);
        await ctx.Articles.AddAsync(article);
        await ctx.SaveChangesAsync();

        return (author, article);
    }

    [Fact]
    public async Task ListArticles_Paged_ShouldReturnCorrectPage()
    {
        var (ctx, svc) = BuildSut();
        var now = DateTime.UtcNow;

        for (var i = 0; i < 10; i++)
            await ctx.Articles.AddAsync(CreateArticle(published: now.AddHours(-i)));
        await ctx.SaveChangesAsync();

        var result = await svc.ListArticles(page: 0, pageSize: 4);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(4);
        result.Value.TotalCount.Should().Be(10);
        result.Value.PageSize.Should().Be(4);
        result.Value.CurrentPage.Should().Be(0);
    }

    [Fact]
    public async Task ListArticles_ShouldNotIncludeSummaries()
    {
        var (ctx, svc) = BuildSut();

        await ctx.Articles.AddRangeAsync(
            CreateArticle(isSummary: false),
            CreateArticle(isSummary: true)
        );
        await ctx.SaveChangesAsync();

        var result = await svc.ListArticles(page: 0, pageSize: 10);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalCount.Should().Be(1);
        result.Value.Items.Should().OnlyContain(a => !a.IsReview);
    }

    // ─────────────────────────────────────────────
    // US-03 | Cikk részletei
    // GetArticleBySlug mindig Include(Author)-t hív és Author.FullName-t olvas,
    // ezért minden tesztnél kell mentett User.
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetArticleBySlug_ShouldReturnCorrectArticleContentAndAuthor()
    {
        var (ctx, svc) = BuildSut();
        const string testSlug = "hungaroring-news-2024";

        var author  = CreateAuthor(username: "Author Alex", fullName: "Csiger Alex");
        var article = CreateArticle(
            author: author, slug: testSlug,
            title:  "F1 at Hungaroring",
            lead:   "The traveling circus arrives at Hungaroring in June"
        );

        await ctx.Users.AddAsync(author);
        await ctx.Articles.AddAsync(article);
        await ctx.SaveChangesAsync();

        var result = await svc.GetArticleBySlug(testSlug);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Title.Should().Be("F1 at Hungaroring");
        result.Value.Slug.Should().Be(testSlug);
        result.Value.Lead.Should().Be("The traveling circus arrives at Hungaroring in June");
        result.Value.FirstSection.Should().NotBeNullOrEmpty();
        result.Value.LastSection.Should().NotBeNullOrEmpty();
        result.Value.IsReview.Should().BeFalse();
        result.Value.AuthorName.Should().Be("Csiger Alex"); // service Author.FullName-t ad vissza
    }

    [Fact]
    public async Task GetArticleBySlug_ShouldReturnFailure_WhenSlugNotFound()
    {
        var (_, svc) = BuildSut();

        var result = await svc.GetArticleBySlug("nonexistent-slug");

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Article not found.");
    }

    [Fact]
    public async Task GetArticleBySlug_Summary_ShouldContainMiddleSections()
    {
        var (ctx, svc) = BuildSut();

        var author  = CreateAuthor();
        var article = CreateArticle(author: author, isSummary: true);
        article.SecondSection = "Second section content here";
        article.ThirdSection  = "Third section content here";
        article.FourthSection = "Fourth section content here";

        await ctx.Users.AddAsync(author);
        await ctx.Articles.AddAsync(article);
        await ctx.SaveChangesAsync();

        var result = await svc.GetArticleBySlug(article.Slug);

        result.IsSuccess.Should().BeTrue();
        result.Value!.IsReview.Should().BeTrue();
        result.Value.MiddleSections.Should().HaveCount(3);
        result.Value.MiddleSections[0].Should().Be("Second section content here");
        result.Value.MiddleSections[1].Should().Be("Third section content here");
        result.Value.MiddleSections[2].Should().Be("Fourth section content here");
    }

    [Fact]
    public async Task GetArticleBySlug_RegularArticle_ShouldHaveNoMiddleSections()
    {
        var (ctx, svc) = BuildSut();

        var (_, article) = await SeedArticleWithAuthor(ctx, isSummary: false);

        var result = await svc.GetArticleBySlug(article.Slug);

        result.IsSuccess.Should().BeTrue();
        result.Value!.MiddleSections.Should().BeEmpty();
    }

    [Fact]
    public async Task GetArticleBySlug_ShouldIncludeImageUrls()
    {
        var (ctx, svc) = BuildSut();

        var (_, article) = await SeedArticleWithAuthor(ctx, slug: "test-slug");

        var result = await svc.GetArticleBySlug(article.Slug);

        result.IsSuccess.Should().BeTrue();
        result.Value!.PrimaryImageUrl.Should().Be("/uploads/images/articles/test-slug/primary.jpg");
        result.Value.LastImageUrl.Should().Be("/uploads/images/articles/test-slug/last.jpg");
    }

    [Fact]
    public async Task GetArticleBySlug_Summary_ShouldIncludeSecondaryAndThirdImageUrl()
    {
        var (ctx, svc) = BuildSut();

        var (_, article) = await SeedArticleWithAuthor(ctx, isSummary: true, slug: "summary-slug");

        var result = await svc.GetArticleBySlug(article.Slug);

        result.IsSuccess.Should().BeTrue();
        result.Value!.SecondaryImageUrl.Should().Be("/uploads/images/articles/summary-slug/secondary.jpg");
        result.Value.ThirdImageUrl.Should().Be("/uploads/images/articles/summary-slug/third.jpg");
    }

    [Fact]
    public async Task GetArticleBySlug_RegularArticle_ShouldHaveNullSecondaryImages()
    {
        var (ctx, svc) = BuildSut();

        var (_, article) = await SeedArticleWithAuthor(ctx, isSummary: false);

        var result = await svc.GetArticleBySlug(article.Slug);

        result.IsSuccess.Should().BeTrue();
        result.Value!.SecondaryImageUrl.Should().BeNull();
        result.Value.ThirdImageUrl.Should().BeNull();
    }

    // ─────────────────────────────────────────────
    // US-04 | Cikk létrehozása
    // ─────────────────────────────────────────────

    [Fact]
    public async Task CreateArticle_ShouldSucceed_WhenAuthorExists()
    {
        var (ctx, svc) = BuildSut();

        var author = CreateAuthor();
        await ctx.Users.AddAsync(author);
        await ctx.SaveChangesAsync();

        var dto = new ArticleCreateDto(
            null, author.Id, "New Title", "new-slug", "F1", false,
            "Lead text for the article that is long enough",
            new string('A', 100), new string('B', 100), null
        );

        var result = await svc.Create(dto);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task CreateArticle_ShouldFail_WhenAuthorNotFound()
    {
        var (_, svc) = BuildSut();

        var dto = new ArticleCreateDto(
            null, Guid.NewGuid(), "T", "S", "F1", false, "Lead", "First", "Last", null
        );

        var result = await svc.Create(dto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Author not found");
    }

    [Fact]
    public async Task CreateArticle_ShouldPersistAllBasicFields()
    {
        var (ctx, svc) = BuildSut();

        var author = CreateAuthor();
        await ctx.Users.AddAsync(author);
        await ctx.SaveChangesAsync();

        var dto = new ArticleCreateDto(
            null, author.Id, "Persisted Title", "persisted-slug", "MotoGP", true,
            "Persisted lead text here for the article",
            new string('F', 100), new string('L', 100), null
        );

        await svc.Create(dto);

        var saved = ctx.Articles.FirstOrDefault(a => a.Slug == "persisted-slug");
        saved.Should().NotBeNull();
        saved!.Title.Should().Be("Persisted Title");
        saved.Tag.Should().Be("MotoGP");
        saved.IsSummary.Should().BeTrue();
        saved.AuthorId.Should().Be(author.Id);
    }

    [Fact]
    public async Task CreateArticle_WithSummary_ShouldPersistMiddleSections()
    {
        var (ctx, svc) = BuildSut();

        var author = CreateAuthor();
        await ctx.Users.AddAsync(author);
        await ctx.SaveChangesAsync();

        var summaryDto = new SummaryDto("Second content", "Third content", "Fourth content");
        var dto = new ArticleCreateDto(
            null, author.Id, "Summary Article", "summary-article-slug", "F1", true,
            "Lead text for a review article",
            new string('F', 100), new string('L', 100), summaryDto
        );

        await svc.Create(dto);

        var saved = ctx.Articles.FirstOrDefault(a => a.Slug == "summary-article-slug");
        saved.Should().NotBeNull();
        saved!.SecondSection.Should().Be("Second content");
        saved.ThirdSection.Should().Be("Third content");
        saved.FourthSection.Should().Be("Fourth content");
    }

    [Fact]
    public async Task CreateArticle_ShouldSetDatePublishedAutomatically()
    {
        var (ctx, svc) = BuildSut();

        var author = CreateAuthor();
        await ctx.Users.AddAsync(author);
        await ctx.SaveChangesAsync();

        var before = DateTime.UtcNow;

        var dto = new ArticleCreateDto(
            null, author.Id, "Dated Article", "dated-article-slug", "F1", false,
            "Lead text here",
            new string('F', 100), new string('L', 100), null
        );

        await svc.Create(dto);

        var saved = ctx.Articles.FirstOrDefault(a => a.Slug == "dated-article-slug");
        saved.Should().NotBeNull();
        saved!.DatePublished.Should().BeOnOrAfter(before);
        saved.DatePublished.Should().BeOnOrBefore(DateTime.UtcNow);
    }

    // ─────────────────────────────────────────────
    // US-05 | Cikk szerkesztése
    // ArticlesRepository.GetById Include(Author)-t hív,
    // ezért Update teszteknél is kell mentett User.
    // ─────────────────────────────────────────────

    [Fact]
    public async Task U_UpdateArticle_ShouldFail_WhenArticleNotFound()
    {
        var (_, svc) = BuildSut();

        var dto = new ArticleUpdateDto(
            Guid.NewGuid(), null, "Title", "Slug", false, "Lead", "First", "Last", null
        );

        var result = await svc.Update(dto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Article not found");
    }

    [Fact]
    public async Task UpdateArticle_ShouldSucceed_AndUpdateAllBasicFields()
    {
        var (ctx, svc) = BuildSut();

        var (_, article) = await SeedArticleWithAuthor(ctx, title: "Original Title");

        var updateDto = new ArticleUpdateDto(
            article.Id, null, "Updated Title", "updated-slug", true,
            "Updated lead text here", new string('U', 100), new string('V', 100),
            new SummaryDto("Sec", "Third", "Fourth")
        );

        var result = await svc.Update(updateDto);

        result.IsSuccess.Should().BeTrue();

        ctx.ChangeTracker.Clear();
        var updated = await ctx.Articles.FindAsync(article.Id);
        updated!.Title.Should().Be("Updated Title");
        updated.Slug.Should().Be("updated-slug");
        updated.IsSummary.Should().BeTrue();
        updated.Lead.Should().Be("Updated lead text here");
        updated.FirstSection.Should().Be(new string('U', 100));
        updated.LastSection.Should().Be(new string('V', 100));
    }
    
    // ─────────────────────────────────────────────
    // US-06 | Cikk törlése
    // Delete → ArticlesRepository.GetById → Include(Author),
    // ezért Delete teszteknél is kell mentett User.
    // ─────────────────────────────────────────────
    
    [Fact]
    public async Task DeleteArticle_ShouldFail_WhenArticleNotFound()
    {
        var (_, svc) = BuildSut();

        var result = await svc.Delete(Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Article not found");
    }
    

    // ─────────────────────────────────────────────
    // US-07 | Összefoglaló (summary/review) lista
    // ─────────────────────────────────────────────

    [Fact]
    public async Task ListAllSummary_ShouldReturnOnlySummaries()
    {
        var (ctx, svc) = BuildSut();

        await ctx.Articles.AddRangeAsync(
            CreateArticle(isSummary: true),
            CreateArticle(isSummary: false),
            CreateArticle(isSummary: true)
        );
        await ctx.SaveChangesAsync();

        var result = await svc.ListAllSummary(page: 0, pageSize: 10);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalCount.Should().Be(2);
        result.Value.Items.Should().OnlyContain(a => a.IsReview);
    }

    [Fact]
    public async Task ListAllSummary_FilteredByTag_ShouldReturnCorrectItems()
    {
        var (ctx, svc) = BuildSut();

        await ctx.Articles.AddRangeAsync(
            CreateArticle(isSummary: true, tag: "F1"),
            CreateArticle(isSummary: true, tag: "MotoGP"),
            CreateArticle(isSummary: true, tag: "F1")
        );
        await ctx.SaveChangesAsync();

        var result = await svc.ListAllSummary(page: 0, pageSize: 10, tag: "F1");

        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalCount.Should().Be(2);
        result.Value.Items.Should().OnlyContain(a => a.Tag == "F1");
    }

    [Fact]
    public async Task ListAllSummary_Paged_ShouldReturnCorrectPage()
    {
        var (ctx, svc) = BuildSut();
        var now = DateTime.UtcNow;

        for (var i = 0; i < 6; i++)
            await ctx.Articles.AddAsync(CreateArticle(isSummary: true, published: now.AddHours(-i)));
        await ctx.SaveChangesAsync();

        var page0 = await svc.ListAllSummary(page: 0, pageSize: 4);
        var page1 = await svc.ListAllSummary(page: 1, pageSize: 4);

        page0.IsSuccess.Should().BeTrue();
        page0.Value!.Items.Should().HaveCount(4);
        page0.Value.TotalCount.Should().Be(6);

        page1.IsSuccess.Should().BeTrue();
        page1.Value!.Items.Should().HaveCount(2);
    }

    // ─────────────────────────────────────────────
    // GetArticleById
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetArticleById_ShouldReturnArticle_WhenExists()
    {
        var (ctx, svc) = BuildSut();

        var author  = CreateAuthor(username: "testuser", fullName: "Test User");
        var article = CreateArticle(author: author);
        await ctx.Users.AddAsync(author);
        await ctx.Articles.AddAsync(article);
        await ctx.SaveChangesAsync();

        var result = await svc.GetArticleById(article.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(article.Id);
        result.Value.AuthorName.Should().Be("Test User"); // service Author.FullName-t mappel
    }

    [Fact]
    public async Task GetArticleById_ShouldReturnFailure_WhenNotFound()
    {
        var (_, svc) = BuildSut();

        var result = await svc.GetArticleById(Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Article not found.");
    }

    // ─────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────

    private static User CreateAuthor(
        string username = "author_user",
        string fullName = "Petofi Sandor") => new()
    {
        Id           = Guid.NewGuid(),
        Username     = username,
        FullName     = fullName,
        Email        = $"{Guid.NewGuid()}@test.com",
        PasswordHash = "pwd",
        Role         = "author"
    };

    private static Article CreateArticle(
        User?     author    = null,
        string?   title     = null,
        string?   slug      = null,
        string?   lead      = null,
        string?   tag       = null,
        bool      isSummary = false,
        DateTime? published = null) => new()
    {
        Id            = Guid.NewGuid(),
        Title         = title     ?? "Article",
        Slug          = slug      ?? Guid.NewGuid().ToString(),
        Lead          = lead      ?? "Lead",
        Tag           = tag       ?? "F1",
        IsSummary     = isSummary,
        FirstSection  = "First section content",
        LastSection   = "Last section content",
        Author        = author,
        AuthorId      = author?.Id ?? Guid.Empty,
        DatePublished = published  ?? DateTime.UtcNow,
        DateUpdated   = DateTime.UtcNow
    };
}
