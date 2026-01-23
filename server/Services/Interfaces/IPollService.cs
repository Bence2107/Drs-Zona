using DTOs.Polls;

namespace Services.Interfaces;

public interface IPollService
{
    ResponseResult<PollDto> GetPollById(int id, int? currentUserId = null);
    ResponseResult<List<PollListDto>> GetPollByCreatorId(int creatorId);
    ResponseResult<List<PollListDto>> GetActivePolls();
    ResponseResult<List<PollListDto>> GetExpiredPolls();
    ResponseResult<List<PollListDto>> ListAllPolls();
    ResponseResult<bool> Create(PollCreateDto dto, int currentUserId);
    ResponseResult<bool> Delete(int id, int currentUserId);
    ResponseResult<bool> Vote(int pollId, int pollOptionId, int userId);
    ResponseResult<bool> RemoveVote(int pollId, int pollOptionId, int userId);
}