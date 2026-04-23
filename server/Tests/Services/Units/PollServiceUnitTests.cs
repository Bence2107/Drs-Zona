using Entities.Models;
using Entities.Models.Polls;
using FluentAssertions;
using Moq;
using Repositories.Interfaces;
using Repositories.Interfaces.Polls;
using Services.Implementations;
using Xunit;

namespace Tests.Services.Units;

public class PollServiceUnitTests
{
    private readonly Mock<IPollsRepository>        _pollRepo    = new();
    private readonly Mock<IPollOptionsRepository>  _optionRepo  = new();
    private readonly Mock<IPollVotesRepository>    _voteRepo    = new();
    private readonly Mock<IAuthRepository>         _userRepo    = new();

    private readonly PollService _svc;

    public PollServiceUnitTests()
    {
        var ctx =
            InMemoryDbFactory.CreateContext();

        _svc = new PollService(
            ctx,
            _pollRepo.Object,
            _optionRepo.Object,
            _voteRepo.Object,
            _userRepo.Object
        );
    }

    // ─────────────────────────────────────────────
    // GetPollById
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetPollById_ShouldFail_WhenPollNotFound()
    {
        var pollId = Guid.NewGuid();
        _pollRepo.Setup(r => r.GetByIdWithAuthor(pollId)).ReturnsAsync((Poll?)null);

        var result = await _svc.GetPollById(pollId);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Szavazás nem található");
    }

    [Fact]
    public async Task GetPollById_ShouldReturnPollDto_WithCorrectFields()
    {
        var poll    = CreatePoll();
        var option1 = CreateOption(poll.Id);
        var option2 = CreateOption(poll.Id);

        _pollRepo.Setup(r => r.GetByIdWithAuthor(poll.Id)).ReturnsAsync(poll);
        _optionRepo.Setup(r => r.GetByPollId(poll.Id)).ReturnsAsync(new List<PollOption> { option1, option2 });
        _voteRepo.Setup(r => r.GetVoteCount(It.IsAny<Guid>())).ReturnsAsync(0);
        _voteRepo.Setup(r => r.GetUserVoteForPoll(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync((PollVote?)null);

        var result = await _svc.GetPollById(poll.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(poll.Id);
        result.Value.Title.Should().Be(poll.Title);
        result.Value.AuthorName.Should().Be(poll.Author!.Username);
        result.Value.PollOptions.Should().HaveCount(2);
        result.Value.TotalVotes.Should().Be(0);
    }

    [Fact]
    public async Task GetPollById_ShouldCalculateVotePercentage_Correctly()
    {
        var poll   = CreatePoll();
        var opt1   = CreateOption(poll.Id);
        var opt2   = CreateOption(poll.Id);

        _pollRepo.Setup(r => r.GetByIdWithAuthor(poll.Id)).ReturnsAsync(poll);
        _optionRepo.Setup(r => r.GetByPollId(poll.Id)).ReturnsAsync(new List<PollOption> { opt1, opt2 });
        _voteRepo.Setup(r => r.GetVoteCount(opt1.Id)).ReturnsAsync(3);
        _voteRepo.Setup(r => r.GetVoteCount(opt2.Id)).ReturnsAsync(1);
        _voteRepo.Setup(r => r.GetUserVoteForPoll(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync((PollVote?)null);

        var result = await _svc.GetPollById(poll.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalVotes.Should().Be(4);
        var opt1Dto = result.Value.PollOptions.First(o => o.Id == opt1.Id);
        opt1Dto.VotePercentage.Should().Be(75);
    }

    [Fact]
    public async Task GetPollById_ShouldMarkUserChoice_WhenUserHasVoted()
    {
        var poll   = CreatePoll();
        var option = CreateOption(poll.Id);
        var userId = Guid.NewGuid();
        var vote   = new PollVote { UserId = userId, PollOptionId = option.Id, PollOption = option };

        _pollRepo.Setup(r => r.GetByIdWithAuthor(poll.Id)).ReturnsAsync(poll);
        _optionRepo.Setup(r => r.GetByPollId(poll.Id)).ReturnsAsync(new List<PollOption> { option });
        _voteRepo.Setup(r => r.GetVoteCount(option.Id)).ReturnsAsync(1);
        _voteRepo.Setup(r => r.GetUserVoteForPoll(userId, poll.Id)).ReturnsAsync(vote);

        var result = await _svc.GetPollById(poll.Id, userId);

        result.IsSuccess.Should().BeTrue();
        result.Value!.UserVoteOptionId.Should().Be(option.Id);
        result.Value.PollOptions.First().IsUserChoice.Should().BeTrue();
    }

    // ─────────────────────────────────────────────
    // GetActivePolls
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetActivePolls_ShouldReturnOnlyNotExpired()
    {
        var activeFuture  = CreatePoll(expiresAt: DateTime.Now.AddDays(1));
        var activeExpired = CreatePoll(expiresAt: DateTime.Now.AddDays(-1));

        _pollRepo.Setup(r => r.GetActive(null))
            .ReturnsAsync(new List<Poll> { activeFuture, activeExpired });

        var result = await _svc.GetActivePolls();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value![0].Id.Should().Be(activeFuture.Id);
    }

    [Fact]
    public async Task GetActivePolls_ShouldReturnEmpty_WhenAllExpired()
    {
        _pollRepo.Setup(r => r.GetActive(null))
            .ReturnsAsync(new List<Poll> { CreatePoll(expiresAt: DateTime.Now.AddDays(-1)) });

        var result = await _svc.GetActivePolls();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    // ─────────────────────────────────────────────
    // GetPollsByCreatorId
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetPollsByCreatorId_ShouldReturnPolls_ForGivenCreator()
    {
        var creatorId = Guid.NewGuid();
        var polls     = new List<Poll> { CreatePoll(authorId: creatorId), CreatePoll(authorId: creatorId) };

        _pollRepo.Setup(r => r.GetByCreatorId(creatorId, null)).ReturnsAsync(polls);

        var result = await _svc.GetPollsByCreatorId(creatorId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    // ─────────────────────────────────────────────
    // Delete
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Delete_ShouldFail_WhenPollNotFound()
    {
        var pollId = Guid.NewGuid();
        _pollRepo.Setup(r => r.GetPollById(pollId)).ReturnsAsync((Poll?)null);

        var result = await _svc.Delete(pollId, Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Szavazás nem található");
    }

    [Fact]
    public async Task Delete_ShouldFail_WhenUserIsNotAuthor()
    {
        var authorId = Guid.NewGuid();
        var poll     = CreatePoll(authorId: authorId);
        _pollRepo.Setup(r => r.GetPollById(poll.Id)).ReturnsAsync(poll);

        var result = await _svc.Delete(poll.Id, Guid.NewGuid()); // más user

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Nem te vagy a szavazás kreátora");
    }

    [Fact]
    public async Task Delete_ShouldSucceed_WhenUserIsAuthor()
    {
        var authorId = Guid.NewGuid();
        var poll     = CreatePoll(authorId: authorId);
        _pollRepo.Setup(r => r.GetPollById(poll.Id)).ReturnsAsync(poll);
        _pollRepo.Setup(r => r.Delete(poll.Id)).Returns(Task.CompletedTask);

        var result = await _svc.Delete(poll.Id, authorId);

        result.IsSuccess.Should().BeTrue();
        _pollRepo.Verify(r => r.Delete(poll.Id), Times.Once);
    }

    // ─────────────────────────────────────────────
    // Vote
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Vote_ShouldFail_WhenPollNotFound()
    {
        _pollRepo.Setup(r => r.GetPollById(It.IsAny<Guid>())).ReturnsAsync((Poll?)null);

        var result = await _svc.Vote(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Szavazás nem található");
    }

    [Fact]
    public async Task Vote_ShouldFail_WhenPollIsNotActive()
    {
        var poll = CreatePoll(isActive: false);
        _pollRepo.Setup(r => r.GetPollById(poll.Id)).ReturnsAsync(poll);

        var result = await _svc.Vote(poll.Id, Guid.NewGuid(), Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Szavazás nem aktív, így nem lehet rá szavazni");
    }

    [Fact]
    public async Task Vote_ShouldFail_WhenPollIsExpired()
    {
        var poll = CreatePoll(expiresAt: DateTime.Now.AddDays(-1));
        _pollRepo.Setup(r => r.GetPollById(poll.Id)).ReturnsAsync(poll);

        var result = await _svc.Vote(poll.Id, Guid.NewGuid(), Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("A szavazás már lejárt");
    }

    [Fact]
    public async Task Vote_ShouldFail_WhenOptionNotFound()
    {
        var poll     = CreatePoll();
        var optionId = Guid.NewGuid();
        _pollRepo.Setup(r => r.GetPollById(poll.Id)).ReturnsAsync(poll);
        _optionRepo.Setup(r => r.GetPollOptionById(optionId)).ReturnsAsync((PollOption?)null);

        var result = await _svc.Vote(poll.Id, optionId, Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Ismeretlen opció");
    }

    [Fact]
    public async Task Vote_ShouldFail_WhenOptionBelongsToDifferentPoll()
    {
        var poll   = CreatePoll();
        var option = CreateOption(Guid.NewGuid()); // más poll-hoz tartozik

        _pollRepo.Setup(r => r.GetPollById(poll.Id)).ReturnsAsync(poll);
        _optionRepo.Setup(r => r.GetPollOptionById(option.Id)).ReturnsAsync(option);

        var result = await _svc.Vote(poll.Id, option.Id, Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Ismeretlen opció");
    }

    [Fact]
    public async Task Vote_ShouldFail_WhenUserAlreadyVoted()
    {
        var userId = Guid.NewGuid();
        var poll   = CreatePoll();
        var option = CreateOption(poll.Id);

        _pollRepo.Setup(r => r.GetPollById(poll.Id)).ReturnsAsync(poll);
        _optionRepo.Setup(r => r.GetPollOptionById(option.Id)).ReturnsAsync(option);
        _optionRepo.Setup(r => r.GetByPollId(poll.Id)).ReturnsAsync(new List<PollOption> { option });
        _voteRepo.Setup(r => r.GetByUserId(userId)).ReturnsAsync(new List<PollVote>
        {
            new() { UserId = userId, PollOptionId = option.Id }
        });

        var result = await _svc.Vote(poll.Id, option.Id, userId);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Már szavaztál ezen a szavazáson");
    }

    [Fact]
    public async Task Vote_ShouldSucceed_AndCallVoteCreate()
    {
        var userId = Guid.NewGuid();
        var poll   = CreatePoll();
        var option = CreateOption(poll.Id);

        _pollRepo.Setup(r => r.GetPollById(poll.Id)).ReturnsAsync(poll);
        _optionRepo.Setup(r => r.GetPollOptionById(option.Id)).ReturnsAsync(option);
        _optionRepo.Setup(r => r.GetByPollId(poll.Id)).ReturnsAsync(new List<PollOption> { option });
        _voteRepo.Setup(r => r.GetByUserId(userId)).ReturnsAsync(new List<PollVote>());
        _voteRepo.Setup(r => r.Create(It.IsAny<PollVote>())).Returns(Task.CompletedTask);

        var result = await _svc.Vote(poll.Id, option.Id, userId);

        result.IsSuccess.Should().BeTrue();
        _voteRepo.Verify(r => r.Create(It.Is<PollVote>(v =>
            v.UserId       == userId &&
            v.PollOptionId == option.Id
        )), Times.Once);
    }

    // ─────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────

    private static Poll CreatePoll(
        Guid?    authorId  = null,
        bool     isActive  = true,
        DateTime? expiresAt = null) => new()
    {
        Id          = Guid.NewGuid(),
        AuthorId    = authorId ?? Guid.NewGuid(),
        Title       = "Test Poll",
        Tag         = "F1",
        Description = "Test description",
        CreatedAt   = DateTime.UtcNow,
        ExpiresAt   = expiresAt ?? DateTime.Now.AddDays(7),
        IsActive    = isActive,
        Author      = new User
        {
            Id           = authorId ?? Guid.NewGuid(),
            Username     = "pollauthor",
            FullName     = "Poll Author",
            Email        = $"{Guid.NewGuid()}@test.com",
            PasswordHash = "pwd",
            Role         = "user"
        }
    };

    private static PollOption CreateOption(Guid pollId) => new()
    {
        Id     = Guid.NewGuid(),
        PollId = pollId,
        Text   = "Option text"
    };
}
