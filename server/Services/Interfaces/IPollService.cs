using DTOs.Polls;
using Services.Types;

namespace Services.Interfaces;

public interface IPollService 
{
    Task<ResponseResult<PollDto>> GetPollById(Guid pollId, Guid? currentUserId = null);
    Task<ResponseResult<List<PollListDto>>> GetPollsByCreatorId(Guid creatorId, string? tag = null);
    Task<ResponseResult<List<PollListDto>>> GetActivePolls(string? tag = null);
    Task<ResponseResult<List<PollListDto>>> GetExpiredPolls(string? tag = null);
    Task<ResponseResult<bool>> Create(PollCreateDto dto, Guid? currentUserId = null);
    Task<ResponseResult<bool>> Delete(Guid id, Guid? currentUserId = null);
    Task<ResponseResult<bool>> Vote(Guid pollId, Guid pollOptionId, Guid userId);
}