using DTOs.RaceTracks;
using Entities.Models.RaceTracks;
using Repositories.Interfaces.RaceTracks;
using Repositories.Interfaces.Standings;
using Services.Interfaces;
using Services.Interfaces.images;
using Services.Types;

namespace Services.Implementations;

public class GrandPrixService (
    ICircuitsRepository circuitsRepo,
    IGrandsPrixRepository grandsPrixRepo,
    ISeriesRepository seriesRepo,
    ICircuitImagesService  circuitImagesService
) : IGrandPrixService 
{
    public async Task<ResponseResult<GrandPrixDetailDto>> GetGrandPrixById(Guid id)
    {
        var grandPrix = await grandsPrixRepo.GetWithAll(id);
        if (grandPrix == null) return ResponseResult<GrandPrixDetailDto>.Failure("Grand Prix not found");
        
        var circuit = grandPrix.Circuit;
        if (circuit == null) return ResponseResult<GrandPrixDetailDto>.Failure("Circuit is not found");
        
        var series = grandPrix.Series;
        if (series == null) return ResponseResult<GrandPrixDetailDto>.Failure("Series is not found");

        var circuitDto = new CircuitDetailDto(
            Id: circuit.Id,
            Name: circuit.Name,
            Length: circuit.Length,
            Type: circuit.Type,
            Location: circuit.Location,
            FastestLap: circuit.FastestLap,
            LightImageUrl: circuitImagesService.GetImageUrl(circuit.Id, "light"),
            DarkImageUrl: circuitImagesService.GetImageUrl(circuit.Id, "dark")
        );

        var detailDto = new GrandPrixDetailDto(
            Id: grandPrix.Id,
            SeriesId: grandPrix.SeriesId,
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
        var circuits = await circuitsRepo.GetAllCircuits();
            
        var dtoS = circuits.Select(c => new CircuitListDto(
            c.Id,
            c.Location,
            c.Name
        )).ToList();

        return ResponseResult<List<CircuitListDto>>.Success(dtoS);
    }

    public async Task<ResponseResult<List<GrandPrixListDto>>> GetSeasonGrandPrixList(Guid seriesId, int year)
    {
        var grandsPrixList = await grandsPrixRepo.GetBySeriesAndYear(seriesId, year);
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

    public async Task<ResponseResult<bool>> Create(GrandPrixCreateDto grandPrixCreateDto)
    {
        if (await circuitsRepo.GetCircuitById(grandPrixCreateDto.CircuitId) == null)
        {
            return ResponseResult<bool>.Failure(nameof(grandPrixCreateDto.CircuitId), "The specified circuit does not exist.");
        }
        
        if (await seriesRepo.GetSeriesById(grandPrixCreateDto.SeriesId) == null)
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

        await grandsPrixRepo.Create(grandPrix);
        return ResponseResult<bool>.Success(true);
    }

    public async Task<ResponseResult<bool>> Update(GrandPrixUpdateDto grandPrixUpdateDto)
    {
        var existing = await grandsPrixRepo.GetGrandPrixById(grandPrixUpdateDto.Id);
        if (existing == null) return ResponseResult<bool>.Failure("Grand Prix not found");

        existing.StartTime = grandPrixUpdateDto.StartTime;
        existing.EndTime = grandPrixUpdateDto.EndTime;
        existing.LapsCompleted = grandPrixUpdateDto.LapsCompleted;

        await grandsPrixRepo.Update(existing);
        return ResponseResult<bool>.Success(true);
    }
}