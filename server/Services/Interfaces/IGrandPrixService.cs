using DTOs.RaceTracks;

namespace Services.Interfaces;

public interface IGrandPrixService
{
    public ResponseResult<GrandPrixDetailDto> GetGrandPrixById(int id);
    ResponseResult<List<GrandPrixListDto>> GetSeasonGrandPrixList(int seriesId, int year);
    
    public ResponseResult<bool> CreateGrandPrix(GrandPrixCreateDto grandPrixCreateDto);
    public ResponseResult<bool> UpdateGrandPrix(GrandPrixUpdateDto grandPrixUpdateDto);
    public ResponseResult<bool> DeleteGrandPrix(int id);
    
}