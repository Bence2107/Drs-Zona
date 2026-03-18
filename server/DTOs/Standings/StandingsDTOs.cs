using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace DTOs.Standings;

public record BrandListDto(
    Guid Id, 
    string? Name
);

public record ConstructorCreateDto(
    [Required(ErrorMessage = "A márka azonosítója kötelező")] 
    Guid BrandId,
    [Required(ErrorMessage = "A név megadása kötelező")] 
    [StringLength(100, ErrorMessage = "A név max 100 karakter")] 
    string Name,
    [Required(ErrorMessage = "A név megadása kötelező")] 
    [StringLength(100, ErrorMessage = "A név max 100 karakter")] 
    string Nickname,
    [Required(ErrorMessage = "Az alapítás éve kötelező")] 
    [Range(1900, 2100, ErrorMessage = "Az alapítás éve 1900 és 2100 között kell legyen")] 
    int FoundedYear,
    [Required(ErrorMessage = "A központ megadása kötelező")] 
    [StringLength(100)] string HeadQuarters,
    [Required(ErrorMessage = "Csapatfőnök megadása kötelező")] 
    [StringLength(100)] string TeamChief,
    [Required(ErrorMessage = "Technikai vezető megadása kötelező")] 
    [StringLength(100)] string TechnicalChief,
    [Required(ErrorMessage = "A bajnoki címek száma kötelező")] 
    [Range(0, 300, ErrorMessage = "Érvénytelen bajnoki cím szám (0-300)")] 
    int Championships,
    [Required(ErrorMessage = "A győzelmek száma kötelező")] 
    [Range(0, 500, ErrorMessage = "Érvénytelen győzelem szám (0-500)")] 
    int Wins,
    [Required(ErrorMessage = "A dobogók száma kötelező")] 
    [Range(0, 1000, ErrorMessage = "Érvénytelen dobogó szám (0-1000)")] 
    int Podiums,
    [Required(ErrorMessage = "A szezonok száma kötelező")] 
    [Range(1, 99, ErrorMessage = "A szezonok száma 1-99 lehet")] 
    int Seasons
);

public record ConstructorUpdateDto(
    [Required(ErrorMessage = "A csapat azonosítója kötelező")] 
    Guid Id,
    [Required(ErrorMessage = "A márka azonosítója kötelező")] 
    Guid BrandId,
    [Required(ErrorMessage = "A név megadása kötelező")] 
    [StringLength(100, ErrorMessage = "A név max 100 karakter")] 
    string Name,
    [Required(ErrorMessage = "A név megadása kötelező")] 
    [StringLength(100, ErrorMessage = "A név max 100 karakter")] 
    string Nickname,
    [Required(ErrorMessage = "Az alapítás éve kötelező")] 
    [Range(1900, 2100, ErrorMessage = "Az alapítás éve 1900 és 2100 között kell legyen")] 
    int FoundedYear,
    [Required(ErrorMessage = "A központ megadása kötelező")] 
    [StringLength(100)] string HeadQuarters,
    [Required(ErrorMessage = "Csapatfőnök megadása kötelező")] 
    [StringLength(100)] string TeamChief,
    [Required(ErrorMessage = "Technikai vezető megadása kötelező")] 
    [StringLength(100)] string TechnicalChief,
    [Required(ErrorMessage = "A bajnoki címek száma kötelező")] 
    [Range(0, 300, ErrorMessage = "Érvénytelen bajnoki cím szám (0-300)")] 
    int Championships,
    [Required(ErrorMessage = "A győzelmek száma kötelező")] 
    [Range(0, 500, ErrorMessage = "Érvénytelen győzelem szám (0-500)")] 
    int Wins,
    [Required(ErrorMessage = "A dobogók száma kötelező")] 
    [Range(0, 1000, ErrorMessage = "Érvénytelen dobogó szám (0-1000)")] 
    int Podiums,
    [Required(ErrorMessage = "A szezonok száma kötelező")] 
    [Range(1, 99, ErrorMessage = "A szezonok száma 1-99 lehet")] 
    int Seasons
);

public record ConstructorListDto(
    Guid Id, 
    string Name
);

public record ConstructorDetailDto(
    Guid Id,
    Guid BrandId,
    string BrandName,
    string BrandDescription,
    string? Name,
    string Nickname,
    int FoundedYear,
    string? HeadQuarters,
    string? TeamChief,
    string? TechnicalChief,  
    List<DriverNameRecord>? DriverNames,
    int TotalWins,
    int TotalPodiums,
    int Championships,
    int Seasons
);

public record DriverCreateDto(
    [Required(ErrorMessage = "A név megadása kötelező")] 
    [StringLength(100, MinimumLength = 2, ErrorMessage = "A név hossza 2 és 100 karakter között lehet")] 
    string Name,
    [Required(ErrorMessage = "A nemzetiség megadása kötelező")] 
    [StringLength(50)] string Nationality,
    [Required(ErrorMessage = "A születési dátum kötelező")] 
    DateTime BirthDate,
    [Required(ErrorMessage = "Az összes verseny száma kötelező")] 
    [Range(0, 500, ErrorMessage = "Az összes verseny száma 0 és 500 között lehet")] 
    int TotalRaces,
    [Required(ErrorMessage = "A győzelmek száma kötelező")] 
    [Range(0, 500, ErrorMessage = "A győzelmek száma 0 és 500 között lehet")] 
    int TotalWins,
    [Required(ErrorMessage = "A dobogók száma kötelező")] 
    [Range(0, 1000, ErrorMessage = "A dobogók száma 0 és 1000 között lehet")] 
    int TotalPodiums,
    [Required(ErrorMessage = "A bajnoki címek száma kötelező")] 
    [Range(0, 99, ErrorMessage = "A bajnoki címek száma 0 és 99 között lehet")] 
    int Championships,
    [Required(ErrorMessage = "A pole pozíciók száma kötelező")] 
    [Range(0, 200, ErrorMessage = "A pole pozíciók száma 0 és 200 között lehet")] 
    int PolePositions,
    [Required(ErrorMessage = "A szezonok száma kötelező")] 
    [Range(1, 30, ErrorMessage = "A szezonok száma 1 és 30 között lehet")] 
    int Seasons
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (BirthDate > DateTime.Today.AddYears(-15))
            yield return new ValidationResult("A versenyzőnek legalább 15 évesnek kell lennie", [nameof(BirthDate)]);

        if (TotalWins > TotalRaces)
            yield return new ValidationResult("A győzelmek száma nem haladhatja meg a versenyek számát", [nameof(TotalWins)]);

        if (TotalPodiums > TotalRaces)
            yield return new ValidationResult("A dobogók száma nem haladhatja meg a versenyek számát", [nameof(TotalPodiums)]);

        if (TotalWins > TotalPodiums)
            yield return new ValidationResult("A győzelmek száma nem haladhatja meg a dobogók számát", [nameof(TotalWins)]);

        if (PolePositions > TotalRaces)
            yield return new ValidationResult("A pole pozíciók száma nem haladhatja meg a versenyek számát", [nameof(PolePositions)]);

        if (Championships > Seasons)
            yield return new ValidationResult("A bajnoki címek száma nem haladhatja meg a szezonok számát", [nameof(Championships)]);
    }
}


public record DriverUpdateDto(
    [Required(ErrorMessage = "Az azonosító megadása kötelező")] 
    Guid Id,
    [Required(ErrorMessage = "A név megadása kötelező")] 
    [StringLength(100, MinimumLength = 2, ErrorMessage = "A név hossza 2 és 100 karakter között lehet")] 
    string Name,
    [Required(ErrorMessage = "A nemzetiség megadása kötelező")] 
    [StringLength(50)] string Nationality,
    [Required(ErrorMessage = "A születési dátum kötelező")] 
    DateTime BirthDate,
    [Required(ErrorMessage = "Az összes verseny száma kötelező")] 
    [Range(0, 500, ErrorMessage = "Az összes verseny száma 0 és 500 között lehet")] 
    int TotalRaces,
    [Required(ErrorMessage = "A győzelmek száma kötelező")] 
    [Range(0, 500, ErrorMessage = "A győzelmek száma 0 és 500 között lehet")] 
    int TotalWins,
    [Required(ErrorMessage = "A dobogók száma kötelező")] 
    [Range(0, 1000, ErrorMessage = "A dobogók száma 0 és 1000 között lehet")] 
    int TotalPodiums,
    [Required(ErrorMessage = "A bajnoki címek száma kötelező")] 
    [Range(0, 99, ErrorMessage = "A bajnoki címek száma 0 és 99 között lehet")] 
    int Championships,
    [Required(ErrorMessage = "A pole pozíciók száma kötelező")] 
    [Range(0, 200, ErrorMessage = "A pole pozíciók száma 0 és 200 között lehet")] 
    int PolePositions,
    [Required(ErrorMessage = "A szezonok száma kötelező")] 
    [Range(1, 30, ErrorMessage = "A szezonok száma 1 és 30 között lehet")] 
    int Seasons
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (BirthDate > DateTime.Today.AddYears(-15))
            yield return new ValidationResult("A versenyzőnek legalább 15 évesnek kell lennie", [nameof(BirthDate)]);
    }
}

public record DriverListDto(
    Guid Id,
    string Name,
    string Nationality,
    int Age,
    string? CurrentTeam
);

public record DriverNameRecord(
    Guid Id,
    string Name
);

public record DriverDetailDto(
    Guid Id,
    string Name,
    string Nationality,
    DateTime BirthDate,
    List<Guid>? ConstructorIds,
    int TotalRaces,
    int TotalWins,
    int TotalPodiums,
    int Championships,
    int PolePositions,
    int Age,
    int Seasons
);

public record ContractCreateDto(
    [Required(ErrorMessage = "Pilóta megadása kötelező")]
    Guid DriverId,
    [Required(ErrorMessage = "Csapat megadása kötelező")]
    Guid TeamId
);

public class ValidSessionAttribute : ValidationAttribute
{
    private static readonly string[] AllowedSessions =
    [
        "Időmérő", "Futam", "Sprint", "Sprint Időmérő"
    ];

    protected override ValidationResult? IsValid(object? value, ValidationContext ctx)
    {
        if (value is not string s) return ValidationResult.Success;
        return AllowedSessions.Contains(s.Trim())
            ? ValidationResult.Success
            : new ValidationResult($"Érvénytelen szakasz. Lehetséges értékek: {string.Join(", ", AllowedSessions)}");
    }
}

public class ValidStatusAttribute : ValidationAttribute
{
    private static readonly string[] AllowedStatuses =
    [
        "Finished", "DNF", "DNS", "DSQ", "DNQ"
    ];

    protected override ValidationResult? IsValid(object? value, ValidationContext ctx)
    {
        if (value is not string s) return ValidationResult.Success;
        return AllowedStatuses.Contains(s.Trim(), StringComparer.OrdinalIgnoreCase)
            ? ValidationResult.Success
            : new ValidationResult("Érvénytelen státusz. Lehetséges értékek: Finished, DNF, DNS, DSQ, DNQ");
    }
}

public record BatchResultCreateDto(
    [Required(ErrorMessage = "Nagydíj megadása kötelező!")]
    Guid GrandPrixId,
    [Required(ErrorMessage = "Egyéni bajnokság megadása kötelező!")]
    Guid DriversChampId,
    [Required(ErrorMessage = "Konstruktőri bajnokság kötelező!")]
    Guid ConsChampId,
    [Required(ErrorMessage = "Szakasz megadása kötelező")]
    [ValidSession]
    string Session,
    List<SingleResultCreateDto> Results
);

public partial class RaceTimeAttribute : ValidationAttribute
{
    private static readonly Regex[] ValidPatterns =
    [
        MyRegex(),
        MyRegex1(),
        MyRegex2(),
        MyRegex3(),
    ];

    protected override ValidationResult? IsValid(object? value, ValidationContext ctx)
    {
        if (value is not string s || s == "-") return ValidationResult.Success;

        return ValidPatterns.Any(p => p.IsMatch(s.Trim()))
            ? ValidationResult.Success
            : new ValidationResult("Érvénytelen időformátum. (pl.: 1:23:45.678 / 1:23.456 / +5.123s / -)");
    }

    [GeneratedRegex(@"^\d+:\d{2}:\d{2}\.\d{3}$")]
    private static partial Regex MyRegex();
    [GeneratedRegex(@"^\d+:\d{2}\.\d{3}$")]
    private static partial Regex MyRegex1();
    [GeneratedRegex(@"^\d+\.\d{3}$")]
    private static partial Regex MyRegex2();
    [GeneratedRegex(@"^\+\d+(\.\d+)?s?$")]
    private static partial Regex MyRegex3();
}

public record SingleResultCreateDto(
    [Required(ErrorMessage = "Pilóta kiválasztása kötelező!")]
    Guid DriverId,
    [Required(ErrorMessage = "Konstruktör kiválasztása kötelező!")]
    Guid ConstructorId,
    [Required(ErrorMessage = "Pozíció megadása kötelező")]
    int FinishPosition,
    [Required(ErrorMessage = "Idő megadása megadása kötelező")]
    [RaceTime]
    string RaceTime, 
    [Required(ErrorMessage = "Körök számának megadása kötelező")] 
    [Range(0, 600)] int LapsCompleted,
    [Required(ErrorMessage = "Státusz megadása kötelező")]
    [ValidStatus]
    string Status,
    bool Pole = false,
    bool IsFastestLap = false, 
    int StartPosition = 0,
    string? Q1 = null,
    string? Q2 = null,
    string? Q3 = null
);

public record ResultEditDto(
    Guid ResultId,
    Guid DriverId,
    int CarNumber,
    string DriverName,
    Guid ConstructorId,
    string ConstructorName,
    [Required(ErrorMessage = "Pozíció megadása kötelező")]
    int FinishPosition,
    [Required(ErrorMessage = "Idő megadása megadása kötelező")]
    [RaceTime]
    string RaceTime, 
    [Required(ErrorMessage = "Körök számának megadása kötelező")] 
    [Range(0, 600)] int LapsCompleted,
    [Required(ErrorMessage = "Státusz megadása kötelező")]
    [ValidStatus]
    string Status,
    bool IsFastestLap, 
    bool IsPole,
    string? Q1 = null,
    string? Q2 = null,
    string? Q3 = null
);

public record SessionEditDto(
    [Required(ErrorMessage = "Nagydíj megadása kötelező")]
    Guid GrandPrixId,
    [Required(ErrorMessage = "Szakasz megadása kötelező")]
    [ValidSession]
    string Session,
    List<ResultEditDto> Results
);

public record SingleResultUpdateDto(
    Guid ResultId,
    [Required(ErrorMessage = "Pozíció megadása kötelező")]
    int FinishPosition,
    [Required(ErrorMessage = "Idő megadása megadása kötelező")]
    [RaceTime]
    string RaceTime, 
    [Required(ErrorMessage = "Körök számának megadása kötelező")] 
    [Range(0, 600)] int LapsCompleted,
    [Required(ErrorMessage = "Státusz megadása kötelező")]
    [ValidStatus]
    string Status,
    bool IsFastestLap,
    bool IsPole,
    string? Q1 = null,
    string? Q2 = null,
    string? Q3 = null
);


public record GrandPrixResultsDto(
    Guid GrandPrixId,
    string Session,
    GrandRrixResultDto[] Results
);


public record GrandRrixResultDto(
    int Position,
    Guid DriverId,
    int DriverNumber,
    string DriverName,
    Guid ConstructorId,
    string ConstructorName,
    string TimeOrCompleted,
    double Points,
    string? Q1,
    string? Q2,
    string? Q3,
    int LapsCompleted,
    bool IsFastestLap = false,
    bool IsPole = false
);

public record GrandPrixChampionshipContextDto(
    Guid DriversChampId,
    Guid ConsChampId,
    string PointSystem,
    List<string> AvailableSessions
);

public record DriverStandingsDto(
    Guid DriverChampId,
    DriverStandingsResultDto[] Results
);

public record DriverStandingsResultDto(
    int Position,
    Guid DriverId,
    string DriverName,
    string Nationality,
    Guid ConstructorId,
    string ConstructorName,
    double Points
);

public record ConstructorStandingsDto(
    Guid ConstructorChampId,
    ConstructorStandingsResultDto[] Results
);

public record ConstructorStandingsResultDto(
    int Position,
    Guid ConstructorId,
    string ConstructorName,
    string ConstructorShortName,
    double Points
);

public record SeriesLookupDto(
    Guid Id, 
    string Name
);

public record YearLookupDto(
    string Season, 
    Guid DriversChampId, 
    Guid ConstructorsChampId
);

public record DriverLookUpDto(
    Guid Id, 
    string Name, 
    Guid? ConstructorId = null
);

public record ConstructorLookUpDto(
    Guid Id, 
    string Name, 
    string ShortName
);

public record GrandPrixLookupDto(
    Guid Id, 
    string Name,
    bool HasResults,
    string CircuitName,
    int RoundNumber,
    string Location,
    DateTime StartTime,
    DateTime Endtime,
    int RaceDistance,
    int LapsCompleted
);

public record DriverSeasonResultDto(
    string GrandPrixName, 
    string GrandPrixShortName, 
    DateTime Date, 
    string TeamName, 
    int Position, 
    double Points
);
public record ConstructorSeasonResultDto(
    string GrandPrixName, 
    string GrandPrixShortName,  
    DateTime Date, 
    double Points
);
public record SeasonOverviewDto(
    string GrandPrixName, 
    string GrandPrixShortName, 
    DateTime Date, 
    string WinnerName, 
    string TeamName, 
    int Laps, 
    string Time
);