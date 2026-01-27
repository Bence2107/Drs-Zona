using DTOs.RaceTracks;

namespace Services.Interfaces;

public interface IGrandPrixService
{
    public ResponseResult<GrandPrixDetailDto> GetGrandPrixById(Guid id);
    ResponseResult<List<GrandPrixListDto>> GetSeasonGrandPrixList(Guid seriesId, int year);
    
    public ResponseResult<bool> CreateGrandPrix(GrandPrixCreateDto grandPrixCreateDto);
    public ResponseResult<bool> UpdateGrandPrix(GrandPrixUpdateDto grandPrixUpdateDto);
    public ResponseResult<bool> DeleteGrandPrix(Guid id);
    
}