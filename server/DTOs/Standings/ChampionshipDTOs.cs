using System.ComponentModel.DataAnnotations;

namespace DTOs.Standings;

public record SeriesListDto(
    Guid Id,
    string Name,
    string ShortName
);

public record SeriesDetailDto(
    Guid Id,
    string Name,
    string ShortName,
    string Description,
    string GoverningBody,
    int FirstYear,
    int LastYear,
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
    [Required(ErrorMessage = "Széria megadása kötelező")]
    Guid SeriesId,
    [Required(ErrorMessage = "Szezon megadása kötelező")]
    [RegularExpression(@"^\d{4}(\/\d{4})?$", ErrorMessage = "A szezon formátuma: 2024 vagy 2024/2025")]
    string Season,
    [Required(ErrorMessage = "Egyéni bajnokság neve kötelező")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "A név hossza 3 és 100 karakter között lehet")]
    string DriversName,
    [Required(ErrorMessage = "Konstruktőri bajnokság neve kötelező")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "A név hossza 3 és 100 karakter között lehet")]
    string ConstructorsName
);

public record ParticipationAddDto(
    [Required(ErrorMessage = "Egyéni bajnokság megadása kötelező")]
    Guid DriversChampId,
    [Required(ErrorMessage = "Konstruktőri bajnokság megadása kötelező")]
    Guid ConstructorsChampId,
    [Required(ErrorMessage = "Legalább egy versenyzőt meg kell adni")]
    [MinLength(1, ErrorMessage = "Legalább egy versenyzőt meg kell adni")]
    List<DriverParticipationRowDto> Drivers,
    [Required(ErrorMessage = "Legalább egy konstruktőrt meg kell adni")]
    [MinLength(1, ErrorMessage = "Legalább egy konstruktőrt meg kell adni")]
    List<Guid> ConstructorIds
);

public abstract record DriverParticipationRowDto(
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