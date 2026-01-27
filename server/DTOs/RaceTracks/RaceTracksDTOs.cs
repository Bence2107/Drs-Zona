using System.ComponentModel.DataAnnotations;

namespace DTOs.RaceTracks;

public record CircuitDetailDto(
    Guid Id,
    string Name,
    int Length,
    string Type,
    string Location,
    string FastestLap
);

public record GrandPrixCreateDto(
    [Required(ErrorMessage = "A pálya kiválasztása kötelező")] 
    Guid CircuitId,
    [Required(ErrorMessage = "A sorozat kiválasztása kötelező")] 
    Guid SeriesId,
    [Required(ErrorMessage = "A név megadása kötelező")] 
    [StringLength(100, ErrorMessage = "A név maximum 100 karakter lehet")] 
    string Name,
    [Required(ErrorMessage = "A forduló száma kötelező")] 
    [Range(1, 30, ErrorMessage = "A forduló 1 és 30 között kell legyen")] 
    int RoundNumber,
    [Required(ErrorMessage = "A szezon éve kötelező")] 
    [Range(1950, 2100, ErrorMessage = "Érvénytelen évszám")] 
    int SeasonYear,
    [Required(ErrorMessage = "A kezdési idő kötelező")] 
    DateTime StartTime,
    [Required(ErrorMessage = "A befejezési idő kötelező")] 
    DateTime EndTime,
    [Required(ErrorMessage = "A versenytáv kötelező")] 
    [Range(1, 1000, ErrorMessage = "A versenytáv 1 és 1000 km között lehet")] 
    int RaceDistance,
    [Range(0, 100, ErrorMessage = "A körök száma 0 és 100 között lehet")] 
    int LapsCompleted
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndTime <= StartTime)
            yield return new ValidationResult("A befejezési időnek a kezdés után kell lennie", [nameof(EndTime)]);
    }
}

public record GrandPrixUpdateDto(
    [Required(ErrorMessage = "Az azonosító kötelező")]
    Guid Id,
    [Required(ErrorMessage = "A kezdési idő kötelező")]
    DateTime StartTime,
    [Required(ErrorMessage = "A befejezési idő kötelező")]
    DateTime EndTime,
    [Range(0, 100, ErrorMessage = "A körök száma 0 és 100 között lehet")] 
    int LapsCompleted
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndTime <= StartTime)
            yield return new ValidationResult("A befejezési időnek a kezdés után kell lennie", [nameof(EndTime)]);
    }
}

public record GrandPrixListDto(
    Guid Id,
    string Name,
    int RoundNumber,
    int SeasonYear,
    DateTime StartTime,
    DateTime EndTime
);

public record GrandPrixDetailDto(
    Guid Id,
    Guid SeriesId,
    Guid CircuitId,
    string Name,
    string SeriesName,
    int RoundNumber,
    int SeasonYear,
    DateTime StartTime,
    DateTime EndTime,
    int RaceDistance,
    int LapsCompleted,
    CircuitDetailDto CircuitDetail
);