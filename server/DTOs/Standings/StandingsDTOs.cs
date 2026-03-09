using System.ComponentModel.DataAnnotations;

namespace DTOs.Standings;

public record BrandCreateDto(
    [Required(ErrorMessage = "Brand nevének megadása kötelező")]
    [StringLength(100, ErrorMessage = "Brand neve nem lehet 100 karakternél több")]
    string Name,
    [Required(ErrorMessage = "Brand leírása kötelező")]
    [StringLength(1000, ErrorMessage = "Brand leírása nem lehet 1000 karakternél több")]
    string Description,
    [Required(ErrorMessage = "Vezető megadása kötelező")]
    [StringLength(100, ErrorMessage = "Brand vezetőjének neve nem lehet 100 karakternél több")]
    string Principal,
    [Required(ErrorMessage = "Bázis megadása kötelező")]
    [StringLength(100, ErrorMessage = "Brand bázisának neve nem lehet 100 karakternél több")]
    string HeadQuarters
);

public record BrandUpdateDto(
    [Required] Guid Id,
    [StringLength(100, ErrorMessage = "Brand neve nem lehet 100 karakternél több")]
    string Name,
    [Required(ErrorMessage = "Brand leírása kötelező")]
    [StringLength(1000, ErrorMessage = "Brand leírása nem lehet 1000 karakternél több")]
    string Description,
    [Required(ErrorMessage = "Vezető megadása kötelező")]
    [StringLength(100, ErrorMessage = "Brand vezetőjének neve nem lehet 100 karakternél több")]
    string Principal,
    [Required(ErrorMessage = "Bázis megadása kötelező")]
    [StringLength(100, ErrorMessage = "Brand bázisának neve nem lehet 100 karakternél több")]
    string HeadQuarters
);

public record BrandListDto(
    [Required] Guid Id,
    [StringLength(100)] string? Name
);

public record BrandDetailDto(
    [Required] Guid Id,
    [StringLength(100)] string Name,
    [StringLength(1000)] string Description,
    [StringLength(100)] string Principal,
    [StringLength(100)] string HeadQuarters
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
    [Required] [Range(0, 300, ErrorMessage = "Érvénytelen bajnoki cím szám")] int Championships,
    [Required] [Range(0, 500, ErrorMessage = "Érvénytelen győzelem szám")] int Wins,
    [Required] [Range(0, 1000, ErrorMessage = "Érvénytelen dobogó szám")] int Podiums,
    [Required] [Range(1, 99, ErrorMessage = "A szezonok száma 1-99 lehet")] int Seasons
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
    [Required] [Range(0, 300, ErrorMessage = "Érvénytelen bajnoki cím szám")] int Championships,
    [Required] [Range(0, 500, ErrorMessage = "Érvénytelen győzelem szám")] int Wins,
    [Required] [Range(0, 1000, ErrorMessage = "Érvénytelen dobogó szám")] int Podiums,
    [Required] [Range(1, 99, ErrorMessage = "A szezonok száma 1-99 lehet")] int Seasons
);

public record ConstructorListDto(
    [Required] Guid Id, 
    [StringLength(100, ErrorMessage = "A név max 100 karakter")] 
    string Name
);

public record ConstructorDetailDto(
    [Required] Guid Id,
    [Required] Guid BrandId,
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
    [Required] [Range(0, 500)] int TotalRaces,
    [Required] [Range(0, 500)] int TotalWins,
    [Required] [Range(0, 1000)] int TotalPodiums,
    [Required] [Range(0, 99)] int Championships,
    [Required] [Range(0, 200)] int PolePositions,
    [Required] [Range(1, 30)] int Seasons
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (BirthDate > DateTime.Today.AddYears(-15))
            yield return new ValidationResult("A versenyzőnek legalább 15 évesnek kell lennie", [nameof(BirthDate)]);
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
    [Required] [Range(0, 500)] int TotalRaces,
    [Required] [Range(0, 500)] int TotalWins,
    [Required] [Range(0, 1000)] int TotalPodiums,
    [Required] [Range(0, 99)] int Championships,
    [Required] [Range(0, 200)] int PolePositions,
    [Required] [Range(1, 30)] int Seasons
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (BirthDate > DateTime.Today.AddYears(-15))
            yield return new ValidationResult("A versenyzőnek legalább 15 évesnek kell lennie", [nameof(BirthDate)]);
    }
}

public record DriverListDto(
    [Required] Guid Id,
    [StringLength(100)] string Name,
    [StringLength(50)] string Nationality,
    [Range(15, 99)] int Age,
    string? CurrentTeam
);

public record DriverNameRecord(
    [Required] Guid Id,
    [StringLength(100)] string Name
);

public record DriverDetailDto(
    Guid Id,
    [Required] [StringLength(100)] string Name,
    [Required] [StringLength(50)] string Nationality,
    [Required] DateTime BirthDate,
    List<Guid>? ConstructorIds,
    [Required] [Range(1, 500)] int TotalRaces,
    [Required] [Range(1, 500)] int TotalWins,
    [Required] [Range(1, 1000)] int TotalPodiums,
    [Required] [Range(1, 99)] int Championships,
    [Required] [Range(1, 200)] int PolePositions,
    int Age,
    int Seasons
);

public record ContractCreateDto(
    Guid DriverId,
    Guid TeamId
);

public record BatchResultCreateDto(
    Guid GrandPrixId,
    Guid DriversChampId,
    Guid ConsChampId,
    string Session,
    List<SingleResultDto> Results
);

public record SingleResultDto(
    Guid DriverId,
    Guid ConstructorId,
    int FinishPosition,
    string RaceTime, 
    int LapsCompleted,
    string Status,
    bool Pole = false,
    int StartPosition = 0
);

public record ResultEditDto(
    Guid ResultId,
    Guid DriverId,
    int CarNumber,
    string DriverName,
    Guid ConstructorId,
    string ConstructorName,
    int StartPosition,
    int FinishPosition,
    string RaceTime, 
    int LapsCompleted,
    string Status,
    double DriverPoints,
    double ConstructorPoints
);

public record SessionEditDto(
    Guid GrandPrixId,
    string Session,
    List<ResultEditDto> Results
);

public record SingleResultUpdateDto(
    Guid ResultId,
    int FinishPosition,
    string RaceTime,
    int LapsCompleted,
    string Status
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
    double Points
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

public record SeriesLookupDto(Guid Id, string Name);
public record YearLookupDto(string Season, Guid DriversChampId, Guid ConstructorsChampId);
public record DriverLookUpDto(Guid Id, string Name, Guid? ConstructorId = null);
public record ConstructorLookUpDto(Guid Id, string Name, string ShortName);
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

public record DefaultFiltersDto(
    Guid SeriesId,
    Guid DriversChampId,
    Guid GrandPrixId,
    string Session
);

public record DriverSeasonResultDto(string GrandPrixName, string GrandPrixShortName, DateTime Date, string TeamName, int Position, double Points);
public record ConstructorSeasonResultDto(string GrandPrixName, string GrandPrixShortName,  DateTime Date, double Points);
public record SeasonOverviewDto(string GrandPrixName, string GrandPrixShortName, DateTime Date, string WinnerName, string TeamName, int Laps, string Time);