using System.ComponentModel.DataAnnotations;

namespace DTOs.Standings;

public record SeriesListDto(
    [Required] Guid Id,
    [Required] [StringLength(100)] string Name
);

public record SeriesCreateDto(
    [Required(ErrorMessage = "Név megadása kötelező")]
    [StringLength(100, ErrorMessage = "A név max 100 karakter")]
    string Name,
    [Required(ErrorMessage = "Leírás megadása kötelező")]
    [StringLength(200, ErrorMessage = "A leírás max 200 karakter")]
    string Description,
    [Required(ErrorMessage = "A birtokos megadása kötelező")]
    [StringLength(100, ErrorMessage = "A birtokos max 100 karakter")]
    string GoverningBody,
    [Required(ErrorMessage = "Az első év megadása kötelező")]
    int FirstYear,
    [Required(ErrorMessage = "Az utolsó év megadása kötelező")]
    int LastYear
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (FirstYear < 1894)
            yield return new ValidationResult("Az első év nem lehet 1894-nél korábbi", [nameof(FirstYear)]);

        if (LastYear > DateTime.Today.Year + 1)
            yield return new ValidationResult("Az utolsó év nem lehet a jövőben", [nameof(LastYear)]);

        if (FirstYear <= LastYear) yield break;
        yield return new ValidationResult("Az időrend nem megfelelő", [nameof(FirstYear)]);
        yield return new ValidationResult("Az időrend nem megfelelő", [nameof(LastYear)]);
    }
}

public record SeriesUpdateDto(
    [Required] Guid Id,
    [Required(ErrorMessage = "Név megadása kötelező")]
    [StringLength(100, ErrorMessage = "A név max 100 karakter")]
    string Name,
    [Required(ErrorMessage = "Leírás megadása kötelező")]
    [StringLength(200, ErrorMessage = "A leírás max 200 karakter")]
    string Description,
    [Required(ErrorMessage = "A birtokos megadása kötelező")]
    [StringLength(100, ErrorMessage = "A birtokos max 100 karakter")]
    string GoverningBody,
    [Required(ErrorMessage = "Az első év megadása kötelező")]
    int FirstYear,
    [Required(ErrorMessage = "Az utolsó év megadása kötelező")]
    int LastYear
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (FirstYear < 1894)
            yield return new ValidationResult("Az első év nem lehet 1894-nél korábbi", [nameof(FirstYear)]);

        if (LastYear > DateTime.Today.Year + 1)
            yield return new ValidationResult("Az utolsó év nem lehet a jövőben", [nameof(LastYear)]);

        if (FirstYear <= LastYear) yield break;
        yield return new ValidationResult("Az időrend nem megfelelő", [nameof(FirstYear)]);
        yield return new ValidationResult("Az időrend nem megfelelő", [nameof(LastYear)]);
    }
}

public record SeriesDetailDto(
    [Required]  Guid Id,
    [Required] [StringLength(100)] string Name,
    [Required] [StringLength(200)] string Description,
    [Required] [StringLength(100)] string GoverningBody,
    [Required] int FirstYear,
    [Required] int LastYear,
    List<string>? AvailableSeasons
);

