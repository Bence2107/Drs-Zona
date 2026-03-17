using Context;
using DTOs.Polls;
using Entities.Models.Polls;
using Repositories.Interfaces;
using Repositories.Interfaces.Polls;
using Services.Interfaces;
using Services.Types;

namespace Services.Implementations;

public class PollService(
    EfContext context,
    IPollsRepository pollRepository,
    IPollOptionsRepository pollOptionsRepository,
    IPollVotesRepository pollVoteRepository,
    IAuthRepository userRepository
) : IPollService
{
    public async Task<ResponseResult<PollDto>> GetPollById(Guid pollId, Guid? currentUserId = null)
    {
        var poll = await pollRepository.GetByIdWithAuthor(pollId);
        if (poll == null) return ResponseResult<PollDto>.Failure("Poll not found");

        var options = await pollOptionsRepository.GetByPollId(pollId);
        
        var totalVotes = 0;
        foreach (var opt in options)
        {
            totalVotes += await pollVoteRepository.GetVoteCount(opt.Id);
        }

        var isExpired = DateTime.Now > poll.ExpiresAt;
        
        Guid? userVoteOptionId = null;
        if (currentUserId.HasValue)
        {
            var userVote = await pollVoteRepository.GetUserVoteForPoll(currentUserId.Value, pollId);
            userVoteOptionId = userVote?.PollOptionId;
        }

        var pollOptionsDto = new List<PollOptionDto>();
        foreach(var o in options)
        {
            var voteCount = await pollVoteRepository.GetVoteCount(o.Id);
            var percentage = totalVotes > 0
                ? Math.Round((double)voteCount / totalVotes * 100, 2)
                : 0;

            pollOptionsDto.Add(new PollOptionDto(
                Id: o.Id,
                Text: o.Text,
                VoteCount: voteCount,
                VotePercentage: percentage,
                IsUserChoice: o.Id == userVoteOptionId
            ));
        }

        return ResponseResult<PollDto>.Success(new PollDto(
            Id: poll.Id,
            AuthorId: poll.AuthorId,
            AuthorName: poll.Author!.Username,
            Title: poll.Title,
            Tag: poll.Tag,
            Description: poll.Description,
            CreatedAt: poll.CreatedAt,
            ExpiresAt: poll.ExpiresAt,
            IsActive: poll.IsActive,
            PollOptions: pollOptionsDto,
            IsExpired: isExpired,
            TotalVotes: totalVotes,
            UserVoteOptionId: userVoteOptionId
        ));
    }

    public async Task<ResponseResult<List<PollListDto>>> GetPollByCreatorId(Guid creatorId, string? tag = null)
    {
        var polls = await pollRepository.GetByCreatorId(creatorId, tag);
        var dto = polls.Select(poll => new PollListDto(
            Id: poll.Id,
            Title: poll.Title,
            Tag: poll.Tag,
            Description: poll.Description,
            ExpiresAt: poll.ExpiresAt
        )).ToList();

        return ResponseResult<List<PollListDto>>.Success(dto);
    }

    public async Task<ResponseResult<List<PollListDto>>> GetActivePolls(string? tag = null)
    {
        var allActivePolls = await pollRepository.GetActive(tag);
        var activePools = allActivePolls
            .Where(p => p.ExpiresAt > DateTime.Now) 
            .ToList();

        var dto = activePools.Select(poll => new PollListDto(
            Id: poll.Id,
            Title: poll.Title,
            Tag: poll.Tag,
            Description: poll.Description,
            ExpiresAt: poll.ExpiresAt
        )).ToList();

        return ResponseResult<List<PollListDto>>.Success(dto);
    }

    public async Task<ResponseResult<List<PollListDto>>> GetExpiredPolls(string? tag = null)
    {
        var expiredPolls = await pollRepository.GetExpired(tag);
        var dto = expiredPolls.Select(poll => new PollListDto(
            Id: poll.Id,
            Title: poll.Title,
            Tag: poll.Tag,
            Description: poll.Description,
            ExpiresAt: poll.ExpiresAt
        )).ToList();

        return ResponseResult<List<PollListDto>>.Success(dto);
    }

    public async Task<ResponseResult<List<PollListDto>>> ListAllPolls(string? tag = null)
    {
        var polls = await pollRepository.GetAll(tag);
        var dto = polls.Select(poll => new PollListDto(
            Id: poll.Id,
            Title: poll.Title,
            Tag: poll.Tag,
            Description: poll.Description,
            ExpiresAt: poll.ExpiresAt
        )).ToList();

        return ResponseResult<List<PollListDto>>.Success(dto);
    }

    public async Task<ResponseResult<bool>> Create(PollCreateDto dto, Guid? currentUserId = null)
    {
        if (currentUserId is not null && !await userRepository.CheckIfIdExists(currentUserId))
            return ResponseResult<bool>.Failure("User not found");

        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var poll = new Poll
            {
                AuthorId = currentUserId,
                Title = dto.Title,
                Tag = dto.Tag,
                Description = dto.Description.Trim(),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = dto.ExpiresAt,
                IsActive = true
            };

            await pollRepository.Add(poll);
            await context.SaveChangesAsync();

            foreach (var optionText in dto.Options)
            {
                var trimmedText = optionText.Trim();
                var option = new PollOption
                {
                    PollId = poll.Id,
                    Text = trimmedText
                };

                await pollOptionsRepository.Create(option);
            }

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            return ResponseResult<bool>.Success(true);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<ResponseResult<bool>> Delete(Guid id, Guid? currentUserId = null)
    {
        var poll = await pollRepository.GetPollById(id);
        if (poll == null)
            return ResponseResult<bool>.Failure("Poll not found");

        if (poll.AuthorId != currentUserId)
        {
            return ResponseResult<bool>.Failure("You are not the author!");
        }
        
        await pollRepository.Delete(id);
        return ResponseResult<bool>.Success(true);
    }
    
    public async Task<ResponseResult<bool>> Vote(Guid pollId, Guid pollOptionId, Guid userId)
    {
        var poll = await pollRepository.GetPollById(pollId);
        if (poll == null)
            return ResponseResult<bool>.Failure("Poll not found");

        if (!poll.IsActive)
            return ResponseResult<bool>.Failure("Poll is not active");

        if (poll.ExpiresAt < DateTime.Now)
            return ResponseResult<bool>.Failure("Poll has expired");

        var option = await pollOptionsRepository.GetPollOptionById(pollOptionId);
        if (option == null || option.PollId != pollId)
            return ResponseResult<bool>.Failure("Invalid poll option");

        if (await HasUserVoted(pollId, userId))
            return ResponseResult<bool>.Failure("User has already voted on this poll");

        var vote = new PollVote
        {
            UserId = userId,
            PollOptionId = pollOptionId
        };

        await pollVoteRepository.Create(vote);
        return ResponseResult<bool>.Success(true);
    }

    public async Task<ResponseResult<bool>> RemoveVote(Guid pollId, Guid pollOptionId, Guid userId)
    {
        var vote = await pollVoteRepository.GetUserVoteForPoll(userId, pollOptionId);
        if (vote == null)
            return ResponseResult<bool>.Failure("Vote not found");

        var option = await pollOptionsRepository.GetPollOptionById(pollOptionId);
        if (option == null || option.PollId != pollId)
            return ResponseResult<bool>.Failure("Poll option not found");

        await pollVoteRepository.Delete(userId, pollOptionId);
        return ResponseResult<bool>.Success(true);
    }
    
    private async Task<bool> HasUserVoted(Guid pollId, Guid userId)
    {
        var options = await pollOptionsRepository.GetByPollId(pollId);
        var optionIds = options.Select(o => o.Id).ToList();

        var userVotes = await pollVoteRepository.GetByUserId(userId);
        return userVotes.Any(v => optionIds.Contains(v.PollOptionId));
    }
}