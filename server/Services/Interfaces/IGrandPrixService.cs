using DTOs.RaceTracks;
using Services.Types;

namespace Services.Interfaces;

public interface IGrandPrixService 
{
    Task<ResponseResult<GrandPrixDetailDto>> GetGrandPrixById(Guid id);
    Task<ResponseResult<List<CircuitListDto>>> GetAllCircuits();
    Task<ResponseResult<List<GrandPrixListDto>>> GetSeasonGrandPrixList(Guid seriesId, int year);
    Task<ResponseResult<bool>> Create(GrandPrixCreateDto grandPrixCreateDto);
    Task<ResponseResult<bool>> Update(GrandPrixUpdateDto grandPrixUpdateDto);
}