using DTOs.Polls;

namespace Services.Interfaces;

public interface IPollService
{
    Task<ResponseResult<PollDto>> GetPollById(Guid pollId, Guid? currentUserId = null);
    Task<ResponseResult<List<PollListDto>>> GetPollByCreatorId(Guid creatorId);
    Task<ResponseResult<List<PollListDto>>> GetActivePolls();
    Task<ResponseResult<List<PollListDto>>> GetExpiredPolls();
    Task<ResponseResult<List<PollListDto>>> ListAllPolls();
    Task<ResponseResult<bool>> Create(PollCreateDto dto, Guid? currentUserId = null);
    Task<ResponseResult<bool>> Delete(Guid id, Guid? currentUserId = null);
    Task<ResponseResult<bool>> Vote(Guid pollId, Guid pollOptionId, Guid userId);
    Task<ResponseResult<bool>> RemoveVote(Guid pollId, Guid pollOptionId, Guid userId);
}