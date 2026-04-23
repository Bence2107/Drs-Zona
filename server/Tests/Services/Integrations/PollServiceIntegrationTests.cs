using Context;
using DTOs.Polls;
using Entities.Models;
using Entities.Models.Polls;
using FluentAssertions;
using Repositories.Implementations;
using Repositories.Implementations.Polls;
using Services.Implementations;
using Services.Interfaces;
using Xunit;

namespace Tests.Services.Integrations;

public class PollServiceIntegrationTests
{

    private static (EfContext ctx, IPollService svc) BuildSut()
    {
        var ctx = InMemoryDbFactory.CreateContext();
        var svc = new PollService(
            ctx,
            new PollsRepository(ctx),
            new PollOptionsRepository(ctx),
            new PollVotesRepository(ctx),
            new AuthRepository(ctx)
        );
        return (ctx, svc);
    }

    // ─────────────────────────────────────────────
    // GetPollById
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetPollById_ShouldFail_WhenPollNotFound()
    {
        var (_, svc) = BuildSut();

        var result = await svc.GetPollById(Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Szavazás nem található");
    }

    [Fact]
    public async Task GetPollById_ShouldReturnPoll_WithOptionsAndVoteCounts()
    {
        var (ctx, svc) = BuildSut();

        var author = CreateUser();
        var poll   = CreatePoll(author.Id);
        var opt1   = CreateOption(poll.Id, "Option A");
        var opt2   = CreateOption(poll.Id, "Option B");

        await ctx.Users.AddAsync(author);
        await ctx.Polls.AddAsync(poll);
        await ctx.PollOptions.AddRangeAsync(opt1, opt2);
        await ctx.SaveChangesAsync();

        // 2 szavazat opt1-re
        await ctx.PollVotes.AddRangeAsync(
            new PollVote { UserId = Guid.NewGuid(), PollOptionId = opt1.Id },
            new PollVote { UserId = Guid.NewGuid(), PollOptionId = opt1.Id }
        );
        await ctx.SaveChangesAsync();

        var result = await svc.GetPollById(poll.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalVotes.Should().Be(2);
        result.Value.PollOptions.Should().HaveCount(2);

        var opt1Dto = result.Value.PollOptions.First(o => o.Id == opt1.Id);
        opt1Dto.VoteCount.Should().Be(2);
        opt1Dto.VotePercentage.Should().Be(100);
    }

    [Fact]
    public async Task GetPollById_ShouldMarkUserChoice_WhenUserHasVoted()
    {
        var (ctx, svc) = BuildSut();

        var author = CreateUser();
        var voter  = CreateUser();
        var poll   = CreatePoll(author.Id);
        var option = CreateOption(poll.Id, "My Choice");

        await ctx.Users.AddRangeAsync(author, voter);
        await ctx.Polls.AddAsync(poll);
        await ctx.PollOptions.AddAsync(option);
        await ctx.SaveChangesAsync();

        await ctx.PollVotes.AddAsync(new PollVote { UserId = voter.Id, PollOptionId = option.Id });
        await ctx.SaveChangesAsync();

        var result = await svc.GetPollById(poll.Id, voter.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value!.UserVoteOptionId.Should().Be(option.Id);
        result.Value.PollOptions.First().IsUserChoice.Should().BeTrue();
    }

    // ─────────────────────────────────────────────
    // GetActivePolls
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetActivePolls_ShouldReturnOnlyActiveFuturePolls()
    {
        var (ctx, svc) = BuildSut();

        var author = CreateUser();
        await ctx.Users.AddAsync(author);

        await ctx.Polls.AddRangeAsync(
            CreatePoll(author.Id, isActive: true,  expiresAt: DateTime.Now.AddDays(2),  title: "Active Future"),
            CreatePoll(author.Id, isActive: true,  expiresAt: DateTime.Now.AddDays(-1), title: "Active Expired"),
            CreatePoll(author.Id, isActive: false, expiresAt: DateTime.Now.AddDays(2),  title: "Inactive Future")
        );
        await ctx.SaveChangesAsync();

        var result = await svc.GetActivePolls();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value![0].Title.Should().Be("Active Future");
    }

    // ─────────────────────────────────────────────
    // GetPollsByCreatorId
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetPollsByCreatorId_ShouldReturnOnlyCreatorPolls()
    {
        var (ctx, svc) = BuildSut();

        var creator1 = CreateUser();
        var creator2 = CreateUser();
        await ctx.Users.AddRangeAsync(creator1, creator2);

        await ctx.Polls.AddRangeAsync(
            CreatePoll(creator1.Id, title: "Creator1 Poll A"),
            CreatePoll(creator1.Id, title: "Creator1 Poll B"),
            CreatePoll(creator2.Id, title: "Creator2 Poll")
        );
        await ctx.SaveChangesAsync();

        var result = await svc.GetPollsByCreatorId(creator1.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().OnlyContain(p => p.Title.StartsWith("Creator1"));
    }

    // ─────────────────────────────────────────────
    // Create
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Create_ShouldFail_WhenUserNotFound()
    {
        var (_, svc) = BuildSut();

        var dto = new PollCreateDto(
            "Test Poll Title Here",
            "Test description text",
            "F1",
            DateTime.Now.AddDays(7),
            ["A", "B"]
        );

        var result = await svc.Create(dto, Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Felhasználó nem található");
    }
    

    // ─────────────────────────────────────────────
    // Delete
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Delete_ShouldSucceed_AndRemovePollFromDb()
    {
        var (ctx, svc) = BuildSut();

        var author = CreateUser();
        var poll   = CreatePoll(author.Id);
        await ctx.Users.AddAsync(author);
        await ctx.Polls.AddAsync(poll);
        await ctx.SaveChangesAsync();

        var result = await svc.Delete(poll.Id, author.Id);

        result.IsSuccess.Should().BeTrue();
        ctx.ChangeTracker.Clear();
        ctx.Polls.Should().BeEmpty();
    }

    [Fact]
    public async Task Delete_ShouldFail_WhenUserIsNotAuthor()
    {
        var (ctx, svc) = BuildSut();

        var author = CreateUser();
        var poll   = CreatePoll(author.Id);
        await ctx.Users.AddAsync(author);
        await ctx.Polls.AddAsync(poll);
        await ctx.SaveChangesAsync();

        var result = await svc.Delete(poll.Id, Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Nem te vagy a szavazás kreátora");
    }

    [Fact]
    public async Task Delete_ShouldFail_WhenPollNotFound()
    {
        var (_, svc) = BuildSut();

        var result = await svc.Delete(Guid.NewGuid(), Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Szavazás nem található");
    }

    // ─────────────────────────────────────────────
    // Vote
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Vote_ShouldSucceed_AndPersistVote()
    {
        var (ctx, svc) = BuildSut();

        var author = CreateUser();
        var voter  = CreateUser();
        var poll   = CreatePoll(author.Id);
        var option = CreateOption(poll.Id, "My Choice");

        await ctx.Users.AddRangeAsync(author, voter);
        await ctx.Polls.AddAsync(poll);
        await ctx.PollOptions.AddAsync(option);
        await ctx.SaveChangesAsync();

        var result = await svc.Vote(poll.Id, option.Id, voter.Id);

        result.IsSuccess.Should().BeTrue();
        ctx.ChangeTracker.Clear();
        ctx.PollVotes.Should().HaveCount(1);
        ctx.PollVotes.First().UserId.Should().Be(voter.Id);
    }

    [Fact]
    public async Task Vote_ShouldFail_WhenAlreadyVoted()
    {
        var (ctx, svc) = BuildSut();

        var author = CreateUser();
        var voter  = CreateUser();
        var poll   = CreatePoll(author.Id);
        var option = CreateOption(poll.Id, "Option A");

        await ctx.Users.AddRangeAsync(author, voter);
        await ctx.Polls.AddAsync(poll);
        await ctx.PollOptions.AddAsync(option);
        await ctx.PollVotes.AddAsync(new PollVote { UserId = voter.Id, PollOptionId = option.Id });
        await ctx.SaveChangesAsync();

        var result = await svc.Vote(poll.Id, option.Id, voter.Id);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Már szavaztál ezen a szavazáson");
    }

    [Fact]
    public async Task Vote_ShouldFail_WhenPollIsExpired()
    {
        var (ctx, svc) = BuildSut();

        var author = CreateUser();
        var poll   = CreatePoll(author.Id, expiresAt: DateTime.Now.AddDays(-1));
        var option = CreateOption(poll.Id, "Option A");

        await ctx.Users.AddAsync(author);
        await ctx.Polls.AddAsync(poll);
        await ctx.PollOptions.AddAsync(option);
        await ctx.SaveChangesAsync();

        var result = await svc.Vote(poll.Id, option.Id, Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("A szavazás már lejárt");
    }

    [Fact]
    public async Task Vote_ShouldFail_WhenPollIsNotActive()
    {
        var (ctx, svc) = BuildSut();

        var author = CreateUser();
        var poll   = CreatePoll(author.Id, isActive: false);
        var option = CreateOption(poll.Id, "Option A");

        await ctx.Users.AddAsync(author);
        await ctx.Polls.AddAsync(poll);
        await ctx.PollOptions.AddAsync(option);
        await ctx.SaveChangesAsync();

        var result = await svc.Vote(poll.Id, option.Id, Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Szavazás nem aktív, így nem lehet rá szavazni");
    }

    // ─────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────

    private static User CreateUser() => new()
    {
        Id           = Guid.NewGuid(),
        Username     = $"user_{Guid.NewGuid():N}",
        FullName     = "Test User",
        Email        = $"{Guid.NewGuid()}@test.com",
        PasswordHash = "pwd",
        Role         = "user"
    };

    private static Poll CreatePoll(
        Guid      authorId  = default,
        bool      isActive  = true,
        DateTime? expiresAt = null,
        string    title     = "Test Poll") => new()
    {
        Id          = Guid.NewGuid(),
        AuthorId    = authorId == default ? Guid.NewGuid() : authorId,
        Title       = title,
        Tag         = "F1",
        Description = "Test description",
        CreatedAt   = DateTime.UtcNow,
        ExpiresAt   = expiresAt ?? DateTime.Now.AddDays(7),
        IsActive    = isActive
    };

    private static PollOption CreateOption(Guid pollId, string text = "Option") => new()
    {
        Id     = Guid.NewGuid(),
        PollId = pollId,
        Text   = text
    };
}
