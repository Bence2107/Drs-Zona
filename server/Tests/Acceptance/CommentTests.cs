using Context;
using DTOs.News;
using Entities.Models;
using Entities.Models.News;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Moq;
using Repositories.Implementations;
using Repositories.Implementations.News;
using Services.Implementations;
using Services.Implementations.image;
using Services.Interfaces;
using Xunit;

namespace Tests.Acceptance;

public class CommentTests
{
    private readonly EfContext _context;
    private readonly ICommentService _service;

    public CommentTests()
    {
        _context = InMemoryDbFactory.CreateContext();
        
        var mockEnv = new Mock<IWebHostEnvironment>();
        var imageService = new UserImageService(mockEnv.Object);

        _service = new CommentService(
            new CommentsRepository(_context),
            new AuthRepository(_context),
           imageService,
            new CommentVotesRepository(_context)
        );
    }

    [Fact]
    public void US_04_AC_01_GetCommentReplies_ShouldReturnAllRepliesForComment()
    {
        var user = CreateUser();
        var parentCommentId = Guid.NewGuid();

        var replies = new List<Comment>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Content = "First Reply",
                ReplyToCommentId = parentCommentId,
                User = user
            },
            new()
            {
                Id = Guid.NewGuid(),
                Content = "Second Reply",
                ReplyToCommentId = parentCommentId,
                User = user
            }
        };

        _context.Users.Add(user);
        _context.Comments.AddRange(replies);
        _context.SaveChanges();

        var result = _service.GetCommentReplies(parentCommentId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should()
            .AllSatisfy(c => c.ReplyToCommentId.Should().Be(parentCommentId));
    }

    [Fact]
    public void GetArticleCommentsWithoutReplies_ShouldReturnComments()
    {
        var user = CreateUser();
        var article = CreateArticle();

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            ArticleId = article.Id,
            Content = "Test comment",
            User = user
        };

        _context.Users.Add(user);
        _context.Articles.Add(article);
        _context.Comments.Add(comment);
        _context.SaveChanges();

        var result = _service.GetArticleCommentsWithoutReplies(article.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }

    [Fact]
    public void GetUsersComments_ShouldReturnComments()
    {
        var user = CreateUser();

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Content = "User comment",
            User = user
        };

        _context.Users.Add(user);
        _context.Comments.Add(comment);
        _context.SaveChanges();

        var result = _service.GetUsersComments(user.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }

    [Fact]
    public void AddComment_ShouldSucceed_WhenArticleExists()
    {
        var user = CreateUser();
        var article = CreateArticle();

        var parentComment = new Comment
        {
            Id = Guid.NewGuid(),
            ArticleId = article.Id,
            UserId = user.Id,
            Content = "Parent comment",
            DateCreated = DateTime.UtcNow
        };

        _context.Users.Add(user);
        _context.Articles.Add(article);
        _context.Comments.Add(parentComment);
        _context.SaveChanges();

        var dto = new CommentCreateDto(
            ArticleId: parentComment.Id,
            Content: "New comment",
            ReplyToCommentId: null
        );

        var result = _service.AddComment(dto, user.Id);

        result.IsSuccess.Should().BeTrue();
        _context.Comments.Should().HaveCount(2);
    }


    [Fact]
    public void AddComment_ShouldFail_WhenArticleDoesNotExist()
    {
        var user = CreateUser();

        _context.Users.Add(user);
        _context.SaveChanges();

        var dto = new CommentCreateDto(
            ArticleId: Guid.NewGuid(),
            Content: "Content",
            ReplyToCommentId: null
        );

        var result = _service.AddComment(dto, user.Id);

        result.IsSuccess.Should().BeFalse();
        _context.Comments.Should().BeEmpty();
    }

    [Fact]
    public void DeleteComment_ShouldSucceed_WhenNoReplies()
    {
        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            Content = "Delete me",
            DateCreated = DateTime.UtcNow
        };

        _context.Comments.Add(comment);
        _context.SaveChanges();

        var result = _service.DeleteComment(comment.Id);

        result.IsSuccess.Should().BeTrue();
        _context.Comments.Should().BeEmpty();
    }
    
    [Fact]
    public void UpdateCommentsVote_ShouldFail_WhenArticleNotFound()
    {
        var dto = new CommentUpdateVoteDto(Guid.NewGuid(), Guid.NewGuid(), true);

        var result = _service.UpdateCommentsVote(dto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Comment not found");
    }

    [Fact]
    public void DeleteComment_ShouldDeleteRepliesAsWell()
    {
        var parentId = Guid.NewGuid();
        var articleId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var parent = new Comment 
        { 
            Id = parentId, 
            Content = "Parent", 
            ArticleId = articleId, 
            UserId = userId,
            DateCreated = DateTime.UtcNow 
        };
    
        var reply = new Comment 
        { 
            Id = Guid.NewGuid(), 
            Content = "Reply", 
            ReplyToCommentId = parentId, 
            ArticleId = articleId, 
            UserId = userId,
            DateCreated = DateTime.UtcNow 
        };

        _context.Comments.AddRange(parent, reply);
        _context.SaveChanges();
    }

    private static User CreateUser() => new()
    {
        Id = Guid.NewGuid(),
        Username = "User",
        FullName = "Lakatos Zsigmond",
        Email = "user@test.com",
        PasswordHash = "pwd",
        Role = "user"
    };

    private static Article CreateArticle() => new()
    {
        Id = Guid.NewGuid(),
        Title = "Article",
        Slug = "article",
        Lead = "Lead",
        FirstSection = "First",
        LastSection = "Last"
    };
}
