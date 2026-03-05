using DTOs.RaceTracks;
using Entities.Models.RaceTracks;
using Repositories.Interfaces.RaceTracks;
using Repositories.Interfaces.Standings;
using Services.Interfaces;

namespace Services.Implementations;

public class GrandPrixService (
    IGrandsPrixRepository grandsPrixRepository,
    ICircuitsRepository circuitsRepository,
    ISeriesRepository seriesRepository)
: IGrandPrixService
{
    public async Task<ResponseResult<GrandPrixDetailDto>> GetGrandPrixById(Guid id)
    {
        var grandPrix = await grandsPrixRepository.GetWithAll(id);
        if (grandPrix == null) return ResponseResult<GrandPrixDetailDto>.Failure("Grand Prix not found");
        
        var circuit = grandPrix.Circuit;
        if (circuit == null) return ResponseResult<GrandPrixDetailDto>.Failure("Circuit is not found");
        
        var series = grandPrix.Series;
        if (series == null) return ResponseResult<GrandPrixDetailDto>.Failure("Series is not found");

        var circuitDto = new CircuitDetailDto(
            Id: id,
            Name: circuit.Name,
            Length: circuit.Length,
            Type: circuit.Type,
            Location: circuit.Location,
            FastestLap: circuit.FastestLap
        );

        var detailDto = new GrandPrixDetailDto(
            Id: grandPrix.Id,
            SeriesId: grandPrix.SeriesId,
            CircuitId: grandPrix.CircuitId,
            Name: grandPrix.Name,
            SeriesName: grandPrix.Series!.Name,
            RoundNumber: grandPrix.RoundNumber,
            SeasonYear: grandPrix.SeasonYear,
            StartTime: grandPrix.StartTime,
            EndTime: grandPrix.EndTime,
            RaceDistance: grandPrix.RaceDistance,
            LapsCompleted: grandPrix.LapsCompleted,
            CircuitDetail: circuitDto 
        );

        return ResponseResult<GrandPrixDetailDto>.Success(detailDto);
    }

    public async Task<ResponseResult<List<CircuitListDto>>> GetAllCircuits()
    {
        var circuits = await circuitsRepository.GetAllCircuits();
            
        var dtoS = circuits.Select(c => new CircuitListDto(
            c.Id,
            c.Location,
            c.Name
        )).ToList();

        return ResponseResult<List<CircuitListDto>>.Success(dtoS);
    }

    public async Task<ResponseResult<List<GrandPrixListDto>>> GetSeasonGrandPrixList(Guid seriesId, int year)
    {
        var grandsPrixList = await grandsPrixRepository.GetBySeriesAndYear(seriesId, year);
        var grandsPrix = grandsPrixList.Select(gp => new GrandPrixListDto(
            Id: gp.Id,
            Name: gp.Name,
            RoundNumber: gp.RoundNumber,
            SeasonYear: gp.SeasonYear,
            StartTime: gp.StartTime,
            EndTime: gp.EndTime
        )).ToList();
        return ResponseResult<List<GrandPrixListDto>>.Success(grandsPrix);
    }

    public async Task<ResponseResult<bool>> CreateGrandPrix(GrandPrixCreateDto grandPrixCreateDto)
    {
        if (await circuitsRepository.GetCircuitById(grandPrixCreateDto.CircuitId) == null)
        {
            return ResponseResult<bool>.Failure(nameof(grandPrixCreateDto.CircuitId), "The specified circuit does not exist.");
        }
        
        if (await seriesRepository.GetSeriesById(grandPrixCreateDto.SeriesId) == null)
        {
            return ResponseResult<bool>.Failure(nameof(grandPrixCreateDto.CircuitId), "The specified series does not exist.");
        }
        
        var grandPrix = new GrandPrix
        {
            CircuitId = grandPrixCreateDto.CircuitId,
            SeriesId = grandPrixCreateDto.SeriesId, 
            Name = grandPrixCreateDto.Name,
            ShortName = grandPrixCreateDto.ShortName,
            RoundNumber = grandPrixCreateDto.RoundNumber,
            SeasonYear = grandPrixCreateDto.SeasonYear,
            StartTime = grandPrixCreateDto.StartTime,
            EndTime = grandPrixCreateDto.EndTime,
            RaceDistance = grandPrixCreateDto.RaceDistance,
            LapsCompleted = grandPrixCreateDto.LapsCompleted
        };

        await grandsPrixRepository.Create(grandPrix);
        return ResponseResult<bool>.Success(true);
    }

    public async Task<ResponseResult<bool>> UpdateGrandPrix(GrandPrixUpdateDto grandPrixUpdateDto)
    {
        var existing = await grandsPrixRepository.GetGrandPrixById(grandPrixUpdateDto.Id);
        if (existing == null) return ResponseResult<bool>.Failure("Grand Prix not found");

        existing.StartTime = grandPrixUpdateDto.StartTime;
        existing.EndTime = grandPrixUpdateDto.EndTime;
        existing.LapsCompleted = grandPrixUpdateDto.LapsCompleted;

        await grandsPrixRepository.Update(existing);
        return ResponseResult<bool>.Success(true);
    }

    public async Task<ResponseResult<bool>> DeleteGrandPrix(Guid id)
    {
        var existing = await grandsPrixRepository.GetGrandPrixById(id);
        if (existing == null) return ResponseResult<bool>.Failure("Grand Prix not found");

        await grandsPrixRepository.Delete(id);
        return ResponseResult<bool>.Success(true);
    }
}