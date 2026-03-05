using DTOs.RaceTracks;

namespace Services.Interfaces;

public interface IGrandPrixService
{
    Task<ResponseResult<GrandPrixDetailDto>> GetGrandPrixById(Guid id);
    Task<ResponseResult<List<CircuitListDto>>> GetAllCircuits();
    Task<ResponseResult<List<GrandPrixListDto>>> GetSeasonGrandPrixList(Guid seriesId, int year);
    Task<ResponseResult<bool>> CreateGrandPrix(GrandPrixCreateDto grandPrixCreateDto);
    Task<ResponseResult<bool>> UpdateGrandPrix(GrandPrixUpdateDto grandPrixUpdateDto);
    Task<ResponseResult<bool>> DeleteGrandPrix(Guid id);
}