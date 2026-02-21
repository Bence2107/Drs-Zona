using Context;
using DTOs.Polls;
using Entities.Models.Polls;
using Repositories.Interfaces;
using Repositories.Interfaces.Polls;
using Services.Interfaces;

namespace Services.Implementations;

public class PollService(
    EfContext context,
    IPollsRepository pollRepository,
    IPollOptionsRepository pollOptionsRepository,
    IVotesRepository voteRepository,
    IAuthRepository userRepository
) : IPollService
{
    public ResponseResult<PollDto> GetPollById(Guid id, Guid? currentUserId = null)
    {
        var poll = pollRepository.GetByIdWithAuthor(id);
        if (poll == null) return ResponseResult<PollDto>.Failure("Poll not found");

        var options = pollOptionsRepository.GetByPollId(id);
        var totalVotes = options.Sum(o => voteRepository.GetVoteCount(o.Id));
        var isExpired = DateTime.Now > poll.ExpiresAt;
        
        Guid? userVoteOptionId = null;
        if (currentUserId.HasValue)
        {
            userVoteOptionId = voteRepository.GetUserVoteForPoll(id, currentUserId.Value)?.PollOptionId;
        }

        return ResponseResult<PollDto>.Success(new PollDto(
            Id: poll.Id,
            AuthorId: poll.AuthorId,
            AuthorName: poll.Author!.Username,
            Description: poll.Description,
            CreatedAt: poll.CreatedAt,
            ExpiresAt: poll.ExpiresAt,
            IsActive: poll.IsActive,
            PollOptions: options.Select(o =>
            {
                var voteCount = voteRepository.GetVoteCount(o.Id);
                var percentage = totalVotes > 0
                    ? Math.Round((double)voteCount / totalVotes * 100, 2)
                    : 0;

                return new PollOptionDto(
                    Id: o.Id,
                    Text: o.Text,
                    VoteCount: voteCount,
                    VotePercentage: percentage,
                    IsUserChoice: o.Id == userVoteOptionId
                );
            }).ToList(),
            IsExpired: isExpired,
            TotalVotes: totalVotes,
            UserVoteOptionId: userVoteOptionId
        ));
    }

    public ResponseResult<List<PollListDto>> GetPollByCreatorId(Guid creatorId)
    {
        var polls = pollRepository.GetByCreatorId(creatorId);
        var dto = polls.Select(poll => new PollListDto(
            Id: poll.Id,
            Title: poll.Title
        )).ToList();

        return ResponseResult<List<PollListDto>>.Success(dto);
    }

    public ResponseResult<List<PollListDto>> GetActivePolls()
    {
        var activePools = pollRepository.GetActive()
            .Where(p => p.ExpiresAt > DateTime.Now) 
            .ToList();
        var dto = activePools.Select(poll => new PollListDto(
            Id: poll.Id,
            Title: poll.Title
        )).ToList();

        return ResponseResult<List<PollListDto>>.Success(dto);
    }

    public ResponseResult<List<PollListDto>> GetExpiredPolls()
    {
        var expiredPolls = pollRepository.GetExpired();
        var dto = expiredPolls.Select(poll => new PollListDto(
            Id: poll.Id,
            Title: poll.Title
        )).ToList();

        return ResponseResult<List<PollListDto>>.Success(dto);
    }

    public ResponseResult<List<PollListDto>> ListAllPolls()
    {
        var polls = pollRepository.GetAll();
        var dto = polls.Select(poll => new PollListDto(
            Id: poll.Id,
            Title: poll.Title
        )).ToList();

        return ResponseResult<List<PollListDto>>.Success(dto);
    }

    public ResponseResult<bool> Create(PollCreateDto dto, Guid currentUserId)
    {
        if (!userRepository.CheckIfIdExists(currentUserId))
            return ResponseResult<bool>.Failure("User not found");

        using var transaction = context.Database.BeginTransaction();

        try
        {
            var poll = new Poll
            {
                AuthorId = currentUserId,
                Title = dto.Title,
                Description = dto.Description.Trim(),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = dto.ExpiresAt,
                IsActive = true
            };

            pollRepository.Add(poll);
            context.SaveChanges();

            foreach (var optionText in dto.Options)
            {
                var trimmedText = optionText.Trim();
                var option = new PollOption
                {
                    PollId = poll.Id,
                    Text = trimmedText
                };

                pollOptionsRepository.Create(option);
            }

            context.SaveChanges();
            transaction.Commit();

            return ResponseResult<bool>.Success(true);
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }

    public ResponseResult<bool> Delete(Guid id, Guid currentUserId)
    {
        var poll = pollRepository.GetPollById(id);
        if (poll == null)
            return ResponseResult<bool>.Failure("Poll not found");

        if (poll.AuthorId != currentUserId)
        {
            return ResponseResult<bool>.Failure("You are not the author!");
        }
        
        pollRepository.Delete(id);
        return ResponseResult<bool>.Success(true);
    }
    
    public ResponseResult<bool> Vote(Guid pollId, Guid pollOptionId, Guid userId)
    {
        var poll = pollRepository.GetPollById(pollId);
        if (poll == null)
            return ResponseResult<bool>.Failure("Poll not found");

        if (!poll.IsActive)
            return ResponseResult<bool>.Failure("Poll is not active");

        if (poll.ExpiresAt < DateTime.Now)
            return ResponseResult<bool>.Failure("Poll has expired");

        var option = pollOptionsRepository.GetPollOptionById(pollOptionId);
        if (option == null || option.PollId != pollId)
            return ResponseResult<bool>.Failure("Invalid poll option");

        if (HasUserVoted(pollId, userId))
            return ResponseResult<bool>.Failure("User has already voted on this poll");

        var vote = new PollVote
        {
            UserId = userId,
            PollOptionId = pollOptionId
        };

        voteRepository.Create(vote);
        return ResponseResult<bool>.Success(true);
    }

    public ResponseResult<bool> RemoveVote(Guid pollId, Guid pollOptionId, Guid userId)
    {
        var vote = voteRepository.GetUserVoteForPoll(userId, pollOptionId);
        if (vote == null)
            return ResponseResult<bool>.Failure("Vote not found");

        var option = pollOptionsRepository.GetPollOptionById(pollOptionId);
        if (option == null || option.PollId != pollId)
            return ResponseResult<bool>.Failure("Poll option not found");

        voteRepository.Delete(userId, pollOptionId);
        return ResponseResult<bool>.Success(true);
    }
    
    private bool HasUserVoted(Guid pollId, Guid userId)
    {
        var optionIds = pollOptionsRepository.GetByPollId(pollId)
            .Select(o => o.Id)
            .ToList();

        var userVotes = voteRepository.GetByUserId(userId);
        return userVotes.Any(v => optionIds.Contains(v.PollOptionId));
    }
}