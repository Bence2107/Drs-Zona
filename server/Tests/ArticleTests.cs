using Entities.Models;
using Entities.Models.News;
using FluentAssertions;
using Moq;
using Repositories.Interfaces;
using Repositories.Interfaces.News;
using Repositories.Interfaces.RaceTracks;
using Services.Implementations;
using Xunit; 

namespace Tests;

public class ArticleTests 
{
    [Fact]
    public void US_03_AC_01_02_GetArticleBySlug_ShouldReturnCorrectArticleContentAndAuthor()
    {
        var mockArticleRepo = new Mock<IArticlesRepository>();
        var mockUserRepo = new Mock<IUsersRepository>();
        var mockGpRepo = new Mock<IGrandsPrixRepository>();
        var service = new ArticleService(mockArticleRepo.Object, mockUserRepo.Object, mockGpRepo.Object);

        const string testSlug = "hungaroring-news-2024";
        var fakeArticle = new Article 
        { 
            Id = Guid.NewGuid(), 
            Title = "F1 at Hungaroring", 
            Slug = testSlug,
            Lead = "The traveling circus arrives at Hungaroring in June",
            FirstSection = "This is the start of the full content...",
            LastSection = "This is the final section of the content",
            IsSummary = false,
            Author = new User
            {
                Id = Guid.NewGuid(),
                Username = "Author Alex",
                Email = "authoralex@gmail.com",
                Password = "Password2424!",
                Role =  "author",
                HasAvatar =  false,
                Created =  DateTime.Now
            }
        };

        mockArticleRepo.Setup(r => r.GetArticleBySlug(testSlug)).Returns(fakeArticle);

        var result = service.GetArticleBySlug(testSlug);

        result.IsSuccess.Should().BeTrue();
        
        result.Value?.Title.Should().Be("F1 at Hungaroring");
        result.Value?.Slug.Should().Be(testSlug);
        result.Value?.Lead.Should().Be("The traveling circus arrives at Hungaroring in June");
        result.Value?.FirstSection.Should().NotBeNullOrEmpty(); 
        result.Value?.LastSection.Should().NotBeNullOrEmpty();
        result.Value?.IsReview.Should().Be(false);
        result.Value?.AuthorName.Should().Be("Author Alex"); 
    }

    [Fact]
    public void US_02_AC_02_GetRecentArticles_ShouldReturnArticles_SortedByDateDescending()
    {
        var mockArticleRepo = new Mock<IArticlesRepository>();
        var mockUserRepo = new Mock<IUsersRepository>();
        var mockGpRepo = new Mock<IGrandsPrixRepository>();
        
        var service = new ArticleService(mockArticleRepo.Object, mockUserRepo.Object, mockGpRepo.Object);

        var now = DateTime.UtcNow;
        var articles = new List<Article>
        {
            new()
            {
                Id = Guid.NewGuid(), Title = "Old", DatePublished = now.AddDays(-5), IsSummary = false,
                Lead = "The weather was very nice long ago", Slug= "old-news", FirstSection = "First section", LastSection = "Last section"
            },
            new()
            {
                Id = Guid.NewGuid(), Title = "Latest", DatePublished = now, IsSummary = false,
                Lead = "The weather was nice today", Slug= "today-news", FirstSection = "First section", LastSection = "Last section"
            },
            new()
            {
                Id = Guid.NewGuid(), Title = "Yesterday", DatePublished = now.AddDays(-1), IsSummary = false,
                Lead = "The weather was nice yesterday", Slug= "yesterday-news", FirstSection = "First section", LastSection = "Last section"
            }
        };

        mockArticleRepo.Setup(r => r.GetRecentNews(It.IsAny<int>()))
            .Returns(articles.OrderByDescending(a => a.DatePublished).ToList());

        var result = service.GetRecentArticles(3);
        result.IsSuccess.Should().BeTrue();
        
        var actualData = result.Value;
        
        actualData.Should().HaveCount(3);
        
        actualData[0].Title.Should().Be("Latest");
        actualData[1].Title.Should().Be("Yesterday");
        actualData[2].Title.Should().Be("Old");

        actualData.Select(a => a.DatePublished).Should().BeInDescendingOrder();
    }
}