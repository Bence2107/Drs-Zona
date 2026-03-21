using DTOs.News;
using Entities.Models;
using Entities.Models.News;
using FluentAssertions;
using Moq;
using Repositories.Interfaces;
using Repositories.Interfaces.News;
using Services.Implementations;
using Services.Interfaces.images;
using Xunit;

namespace Tests.Services.Units;

public class CommentServiceUnitTests
{

    private readonly Mock<IArticlesRepository>    _articleRepo      = new();
    private readonly Mock<ICommentsRepository>    _commentRepo      = new();
    private readonly Mock<IAuthRepository>        _userRepo         = new();
    private readonly Mock<IUserImageService>      _userImageService = new();
    private readonly Mock<ICommentVotesRepository> _voteRepo        = new();

    private readonly CommentService _svc;

    public CommentServiceUnitTests()
    {
        _userImageService
            .Setup(s => s.GetAvatarUrl(It.IsAny<Guid>()))
            .ReturnsAsync((Guid id) => $"/uploads/avatars/{id}.jpg");

        _svc = new CommentService(
            _articleRepo.Object,
            _commentRepo.Object,
            _userRepo.Object,
            _userImageService.Object,
            _voteRepo.Object
        );
    }

    // ─────────────────────────────────────────────
    // Create
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Create_ShouldFail_WhenUserNotFound()
    {
        var userId = Guid.NewGuid();
        _userRepo.Setup(r => r.CheckIfIdExists(userId)).ReturnsAsync(false);

        var dto = new CommentCreateDto(Guid.NewGuid(), null, "Content");

        var result = await _svc.Create(dto, userId);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("User not found");
    }

    [Fact]
    public async Task Create_ShouldFail_WhenArticleNotFound()
    {
        var userId    = Guid.NewGuid();
        var articleId = Guid.NewGuid();

        _userRepo.Setup(r => r.CheckIfIdExists(userId)).ReturnsAsync(true);
        _articleRepo.Setup(r => r.GetById(articleId)).ReturnsAsync((Article?)null);

        var dto = new CommentCreateDto(articleId, null, "Content");

        var result = await _svc.Create(dto, userId);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Article not found");
    }

    [Fact]
    public async Task Create_ShouldSucceed_AndCallCommentRepoCreate()
    {
        var userId    = Guid.NewGuid();
        var articleId = Guid.NewGuid();

        _userRepo.Setup(r => r.CheckIfIdExists(userId)).ReturnsAsync(true);
        _articleRepo.Setup(r => r.CheckIfIdExists(articleId)).ReturnsAsync(true);
        _commentRepo.Setup(r => r.Create(It.IsAny<Comment>())).Returns(Task.CompletedTask);

        var dto = new CommentCreateDto(articleId, null, "Hello!");

        var result = await _svc.Create(dto, userId);

        result.IsSuccess.Should().BeTrue();
        _commentRepo.Verify(r => r.Create(It.Is<Comment>(c =>
            c.UserId    == userId    &&
            c.ArticleId == articleId &&
            c.Content   == "Hello!"
        )), Times.Once);
    }

    [Fact]
    public async Task Create_ShouldSetReplyToCommentId_WhenProvided()
    {
        var userId    = Guid.NewGuid();
        var articleId = Guid.NewGuid();
        var replyToId = Guid.NewGuid();
        Comment? saved = null;

        _userRepo.Setup(r => r.CheckIfIdExists(userId)).ReturnsAsync(true);
        _articleRepo.Setup(r => r.CheckIfIdExists(articleId)).ReturnsAsync(true);
        _commentRepo
            .Setup(r => r.Create(It.IsAny<Comment>()))
            .Callback<Comment>(c => saved = c)
            .Returns(Task.CompletedTask);
        _commentRepo.Setup(r => r.GetCommentById(replyToId)).ReturnsAsync(CreateComment(replyToId));

        var dto = new CommentCreateDto(articleId, replyToId, "Reply content");

        await _svc.Create(dto, userId);

        saved!.ReplyToCommentId.Should().Be(replyToId);
    }

    // ─────────────────────────────────────────────
    // DeleteComment
    // ─────────────────────────────────────────────

    [Fact]
    public async Task DeleteComment_ShouldFail_WhenCommentNotFound()
    {
        var commentId = Guid.NewGuid();
        _commentRepo.Setup(r => r.GetCommentById(commentId)).ReturnsAsync((Comment?)null);

        var result = await _svc.DeleteComment(commentId);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Comment not found");
    }

    [Fact]
    public async Task DeleteComment_ShouldSucceed_WhenNoReplies()
    {
        var comment = CreateComment();

        _commentRepo.Setup(r => r.GetCommentById(comment.Id)).ReturnsAsync(comment);
        _commentRepo.Setup(r => r.GetRepliesToAComment(comment.Id)).ReturnsAsync(new List<Comment>());
        _commentRepo.Setup(r => r.Delete(comment.Id)).Returns(Task.CompletedTask);

        var result = await _svc.DeleteComment(comment.Id);

        result.IsSuccess.Should().BeTrue();
        _commentRepo.Verify(r => r.Delete(comment.Id), Times.Once);
    }

    [Fact]
    public async Task DeleteComment_ShouldDeleteRepliesFirst_ThenParent()
    {
        var parent = CreateComment();
        var reply1 = CreateComment(replyToId: parent.Id);
        var reply2 = CreateComment(replyToId: parent.Id);

        _commentRepo.Setup(r => r.GetCommentById(parent.Id)).ReturnsAsync(parent);
        _commentRepo.Setup(r => r.GetRepliesToAComment(parent.Id))
            .ReturnsAsync(new List<Comment> { reply1, reply2 });
        _commentRepo.Setup(r => r.Delete(It.IsAny<Guid>())).Returns(Task.CompletedTask);

        var result = await _svc.DeleteComment(parent.Id);

        result.IsSuccess.Should().BeTrue();
        _commentRepo.Verify(r => r.Delete(reply1.Id), Times.Once);
        _commentRepo.Verify(r => r.Delete(reply2.Id), Times.Once);
        _commentRepo.Verify(r => r.Delete(parent.Id), Times.Once);
    }

    // ─────────────────────────────────────────────
    // UpdateCommentsContent
    // ─────────────────────────────────────────────

    [Fact]
    public async Task UpdateCommentsContent_ShouldFail_WhenCommentNotFound()
    {
        _commentRepo.Setup(r => r.GetCommentById(It.IsAny<Guid>())).ReturnsAsync((Comment?)null);

        var dto = new CommentContentUpdateDto(Guid.NewGuid(), Guid.NewGuid(), "New content");

        var result = await _svc.UpdateCommentsContent(dto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("A komment nem található");
    }

    [Fact]
    public async Task UpdateCommentsContent_ShouldFail_WhenArticleIdMismatch()
    {
        var comment = CreateComment();
        _commentRepo.Setup(r => r.GetCommentById(comment.Id)).ReturnsAsync(comment);

        // Más articleId-t adunk mint ami a comment-en van
        var dto = new CommentContentUpdateDto(comment.Id, Guid.NewGuid(), "New content");

        var result = await _svc.UpdateCommentsContent(dto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Nem megfelelő hír");
    }

    [Fact]
    public async Task UpdateCommentsContent_ShouldSucceed_AndUpdateContent()
    {
        var articleId = Guid.NewGuid();
        var comment   = CreateComment(articleId: articleId);
        Comment? updated = null;

        _commentRepo.Setup(r => r.GetCommentById(comment.Id)).ReturnsAsync(comment);
        _commentRepo
            .Setup(r => r.Update(It.IsAny<Comment>()))
            .Callback<Comment>(c => updated = c)
            .Returns(Task.CompletedTask);

        var dto = new CommentContentUpdateDto(comment.Id, articleId, "Updated content");

        var result = await _svc.UpdateCommentsContent(dto);

        result.IsSuccess.Should().BeTrue();
        updated!.Content.Should().Be("Updated content");
    }

    // ─────────────────────────────────────────────
    // UpdateCommentsVote
    // ─────────────────────────────────────────────

    [Fact]
    public async Task UpdateCommentsVote_ShouldFail_WhenCommentNotFound()
    {
        _commentRepo.Setup(r => r.GetCommentById(It.IsAny<Guid>())).ReturnsAsync((Comment?)null);

        var dto = new CommentUpdateVoteDto(Guid.NewGuid(), Guid.NewGuid(), true);

        var result = await _svc.UpdateCommentsVote(dto);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("A komment nem található.");
    }

    [Fact]
    public async Task UpdateCommentsVote_ShouldAddUpvote_WhenNoExistingVote()
    {
        var userId    = Guid.NewGuid();
        var comment   = CreateComment();
        Comment? saved = null;

        _commentRepo.Setup(r => r.GetCommentById(comment.Id)).ReturnsAsync(comment);
        _voteRepo.Setup(r => r.GetVoteForACommment(userId, comment.Id)).ReturnsAsync((CommentVote?)null);
        _voteRepo.Setup(r => r.Create(It.IsAny<CommentVote>())).Returns(Task.CompletedTask);
        _commentRepo
            .Setup(r => r.Update(It.IsAny<Comment>()))
            .Callback<Comment>(c => saved = c)
            .Returns(Task.CompletedTask);

        var dto = new CommentUpdateVoteDto(userId, comment.Id, true);

        var result = await _svc.UpdateCommentsVote(dto);

        result.IsSuccess.Should().BeTrue();
        saved!.UpVotes.Should().Be(1);
        saved.DownVotes.Should().Be(0);
    }

    [Fact]
    public async Task UpdateCommentsVote_ShouldRemoveVote_WhenSameVoteExists()
    {
        var userId  = Guid.NewGuid();
        var comment = CreateComment();
        comment.UpVotes = 1;
        Comment? saved = null;

        var existingVote = new CommentVote { UserId = userId, CommentId = comment.Id, IsUpvote = true };

        _commentRepo.Setup(r => r.GetCommentById(comment.Id)).ReturnsAsync(comment);
        _voteRepo.Setup(r => r.GetVoteForACommment(userId, comment.Id)).ReturnsAsync(existingVote);
        _voteRepo.Setup(r => r.Delete(existingVote)).Returns(Task.CompletedTask);
        _commentRepo
            .Setup(r => r.Update(It.IsAny<Comment>()))
            .Callback<Comment>(c => saved = c)
            .Returns(Task.CompletedTask);

        var dto = new CommentUpdateVoteDto(userId, comment.Id, true);

        var result = await _svc.UpdateCommentsVote(dto);

        result.IsSuccess.Should().BeTrue();
        saved!.UpVotes.Should().Be(0);
    }

    [Fact]
    public async Task UpdateCommentsVote_ShouldSwitchVote_WhenOppositeVoteExists()
    {
        var userId  = Guid.NewGuid();
        var comment = CreateComment();
        comment.DownVotes = 1;
        Comment? saved = null;

        var existingVote = new CommentVote { UserId = userId, CommentId = comment.Id, IsUpvote = false };

        _commentRepo.Setup(r => r.GetCommentById(comment.Id)).ReturnsAsync(comment);
        _voteRepo.Setup(r => r.GetVoteForACommment(userId, comment.Id)).ReturnsAsync(existingVote);
        _voteRepo.Setup(r => r.Update(existingVote)).Returns(Task.CompletedTask);
        _commentRepo
            .Setup(r => r.Update(It.IsAny<Comment>()))
            .Callback<Comment>(c => saved = c)
            .Returns(Task.CompletedTask);

        var dto = new CommentUpdateVoteDto(userId, comment.Id, true);

        var result = await _svc.UpdateCommentsVote(dto);

        result.IsSuccess.Should().BeTrue();
        saved!.UpVotes.Should().Be(1);
        saved.DownVotes.Should().Be(0);
    }

    // ─────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────

    private static Article CreateArticle(Guid? id = null) => new()
    {
        Id           = id ?? Guid.NewGuid(),
        Title        = "Article",
        Slug         = "article-slug",
        Lead         = "Lead",
        Tag          = "F1",
        FirstSection = "First",
        LastSection  = "Last"
    };

    private static Comment CreateComment(
        Guid?  replyToId = null,
        Guid?  articleId = null) => new()
    {
        Id               = Guid.NewGuid(),
        UserId           = Guid.NewGuid(),
        ArticleId        = articleId ?? Guid.NewGuid(),
        ReplyToCommentId = replyToId,
        Content          = "Test comment",
        UpVotes          = 0,
        DownVotes        = 0,
        DateCreated      = DateTime.UtcNow,
        DateUpdated      = DateTime.UtcNow,
        User             = new User
        {
            Id           = Guid.NewGuid(),
            Username     = "testuser",
            FullName     = "Test User",
            Email        = $"{Guid.NewGuid()}@test.com",
            PasswordHash = "pwd",
            Role         = "user"
        }
    };
}