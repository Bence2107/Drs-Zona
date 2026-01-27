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
    Guid Id,
    [StringLength(100)] string? Name
);

public record BrandDetailDto(
    Guid Id,
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
    [Required] [Range(0, 500, ErrorMessage = "Érvénytelen dobogó szám")] int Podiums,
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
    [Required] [Range(0, 500, ErrorMessage = "Érvénytelen dobogó szám")] int Podiums,
    [Required] [Range(1, 99, ErrorMessage = "A szezonok száma 1-99 lehet")] int Seasons
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
    int FoundedYear,
    string? HeadQuarters,
    string? TeamChief,
    string? TechnicalChief,  
    List<DriverNameRecord>? DriverNames,
    int TotalWins,
    int TotalPodiums,
    int Championships
);

public record DriverCreateDto(
    [Required(ErrorMessage = "A név megadása kötelező")] 
    [StringLength(100, MinimumLength = 2, ErrorMessage = "A név hossza 2 és 100 karakter között lehet")] 
    string Name,
    [Required(ErrorMessage = "A nemzetiség megadása kötelező")] 
    [StringLength(50)] string Nationality,
    [Required(ErrorMessage = "A születési dátum kötelező")] 
    DateTime BirthDate,
    [Required(ErrorMessage = "A rajtszám kötelező")] 
    [Range(1, 99, ErrorMessage = "A rajtszám 1 és 99 között lehet")] 
    int DriverNumber,
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
    [Required(ErrorMessage = "A rajtszám kötelező")] 
    [Range(1, 99, ErrorMessage = "A rajtszám 1 és 99 között lehet")] 
    int DriverNumber,
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
    Guid Id,
    [StringLength(100)] string Name,
    [StringLength(50)] string Nationality,
    [Range(1, 99)] int DriverNumber,
    [Range(15, 99)] int Age,
    string? CurrentTeam
);

public record DriverNameRecord(
    Guid Id,
    [StringLength(100)] string Name
);

public record DriverDetailDto(
    Guid Id,
    [Required] [StringLength(100)] string Name,
    [Required] [StringLength(50)] string Nationality,
    [Required] DateTime BirthDate,
    [Required] [Range(1, 99)] int DriverNumber,
    List<Guid>? ConstructorIds,
    [Required] [Range(1, 500)] int TotalRaces,
    [Required] [Range(1, 500)] int TotalWins,
    [Required] [Range(1, 1000)] int TotalPodiums,
    [Required] [Range(1, 99)] int Championships,
    [Required] [Range(1, 200)] int PolePositions,
    int Age
);

public record ContractCreateDto(
    Guid DriverId,
    Guid TeamId
);

public record ContractDto(int Id, int DriverId, string DriverName, int TeamId, string TeamName);

public record ResultCreateDto(
    [Required(ErrorMessage = "Nagydíj azonosító kötelező")] 
    Guid GrandPrixId,
    [Required(ErrorMessage = "Versenyző azonosító kötelező")] 
    Guid DriverId,
    [Required(ErrorMessage = "Konstruktőr azonosító kötelező")] 
    Guid ConstructorId,
    Guid DriversChampId,
    Guid ConsChampId,
    [Required(ErrorMessage = "Rajthely kötelező")] 
    [Range(1, 30, ErrorMessage = "A rajthely 1-30 között lehet")] int StartPosition,
    [Required(ErrorMessage = "Helyezés kötelező")] 
    [Range(1, 30, ErrorMessage = "A helyezés 1-30 között lehet")] int FinishPosition,
    [Required(ErrorMessage = "Munkamenet típus kötelező")]
    [RegularExpression("^(Race|Qualifying|Sprint|Practice)$", ErrorMessage = "Érvénytelen típus (Race, Qualifying, Sprint, Practice)")]
    string Session,
    [Required] [Range(0, long.MaxValue, ErrorMessage = "Érvénytelen időeredmény")] long RaceTime,
    [Required] [Range(0, 26, ErrorMessage = "A pontszám 0-26 lehet")] int DriverPoints,
    [Required] [Range(0, 44, ErrorMessage = "A pontszám 0-44 lehet")] int ConstructorPoints
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Session is not ("Race" or "Sprint") && DriverPoints > 0)
            yield return new ValidationResult("Pont csak Race vagy Sprint munkamenetben szerezhető!", [nameof(DriverPoints)]);
    }
}

public record ResultUpdateDto(
    [Required]
    [Range(0, 26, ErrorMessage = "A pontszám 0-26 lehet")]
    int DriverPoints,
    [Required]
    [Range(0, 44, ErrorMessage = "A pontszám 0-44 lehet")]
    int ConstructorPoints
);

public record ResultDto(
    Guid Id,
    Guid GrandPrixId,
    string GrandPrixName,
    Guid DriverId,
    string DriverName,
    int DriverNumber,
    Guid ConstructorId,
    string ConstructorName,
    int StartPosition,
    int FinishPosition,
    string Session,
    string RaceTimeFormatted,
    int DriverPoints,
    int ConstructorPoints
);

public record DriversStandingsDto(
    int Position,
    Guid DriverId,
    string DriverName,
    int DriverNumber,
    string Nationality,
    Guid TeamId,
    string TeamName,
    int Points,
    int Wins,
    int Podiums,
    int PolePositions,
    int FastestLaps
);

public record ConstructorsStandingsDto(
    int Position,
    Guid ConstructorId,
    string ConstructorName,
    string BrandName,
    int Points,
    int Wins,
    int Podiums
);