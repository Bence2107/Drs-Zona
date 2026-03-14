using System.ComponentModel.DataAnnotations;

namespace DTOs.Standings;

public record SeriesListDto(
    [Required] Guid Id,
    [Required] [StringLength(100)] string Name,
    [Required] [StringLength(10)] string ShortName
);

public record SeriesCreateDto(
    [Required] string Name,
    string ShortName,
    [Required] string Description,
    [Required] string GoverningBody,
    [Required] int FirstYear,
    [Required] int LastYear,
    [Required] string PointSystem
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (FirstYear > LastYear)
            yield return new ValidationResult("Az időrend nem megfelelő", [nameof(FirstYear)]);
    }
}

public record SeriesUpdateDto(
    [Required] Guid Id,
    [Required] string Name,
    string ShortName,
    [Required] string Description,
    [Required] string GoverningBody,
    [Required] int FirstYear,
    [Required] int LastYear,
    [Required] string PointSystem
);

public record SeriesDetailDto(
    [Required] Guid Id,
    [Required] [StringLength(100)] string Name,
    [Required] [StringLength(100)] string ShortName,
    [Required] [StringLength(200)] string Description,
    [Required] [StringLength(100)] string GoverningBody,
    [Required] int FirstYear,
    [Required] int LastYear,
    List<string>? AvailableSeasons
);

public record ChampionshipRowDto(
    Guid DriversChampId,
    Guid ConstructorsChampId,

    string Season,
    string Status,
    string SeriesName,

    string DriversChampName,
    string ConstructorsChampName
);

public record ChampionshipCreateDto(
    Guid SeriesId,
    string Season,
    string DriversName,
    string ConstructorsName
);

public record AddParticipationsDto(
    Guid DriversChampId,
    Guid ConstructorsChampId,
    List<DriverParticipationRowDto> Drivers,
    List<Guid> ConstructorIds
);

public record DriverParticipationRowDto(
    Guid DriverId,
    int DriverNumber
);

public record ParticipationListDto(
    Guid DriversChampId,
    Guid ConstructorsChampId,
    List<DriverParticipationDto> Drivers,
    List<ConstructorListDto> Constructors
);

public record DriverParticipationDto(
    Guid DriverId,
    string Name,
    string Nationality,
    int Age,
    int DriverNumber,
    string? TeamName
);

public record ContractListDto(
    Guid Id,
    Guid DriverId,
    string DriverName,
    Guid TeamId,
    string TeamName
);