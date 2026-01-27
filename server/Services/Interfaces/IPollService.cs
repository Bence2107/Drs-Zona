using DTOs.Polls;

namespace Services.Interfaces;

public interface IPollService
{
    ResponseResult<PollDto> GetPollById(Guid id, Guid? currentUserId = null);
    ResponseResult<List<PollListDto>> GetPollByCreatorId(Guid creatorId);
    ResponseResult<List<PollListDto>> GetActivePolls();
    ResponseResult<List<PollListDto>> GetExpiredPolls();
    ResponseResult<List<PollListDto>> ListAllPolls();
    ResponseResult<bool> Create(PollCreateDto dto, Guid currentUserId);
    ResponseResult<bool> Delete(Guid id, Guid currentUserId);
    ResponseResult<bool> Vote(Guid pollId, Guid pollOptionId, Guid userId);
    ResponseResult<bool> RemoveVote(Guid pollId, Guid pollOptionId, Guid userId);
}