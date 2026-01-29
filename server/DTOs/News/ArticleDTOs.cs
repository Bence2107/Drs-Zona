using System.ComponentModel.DataAnnotations;

namespace DTOs.News;

public record ArticleCreateDto(
    Guid? GrandPrixId,
    [Required(ErrorMessage = "Az író megadása kötelező")]
    Guid AuthorId,
    [Required(ErrorMessage = "A cím kitöltése kötelező")]
    [StringLength(200, MinimumLength = 5, ErrorMessage = "A cím hossza nem megfelelő (5-200 karakter)")]
    string Title,
    [StringLength(200, MinimumLength = 5, ErrorMessage = "A slug hossza nem megfelelő (5-200 karakter)")]
    string Slug,
    [Required(ErrorMessage = "Kötelező megadni a hír összefoglaló-e?")]
    bool IsReview,
    [Required(ErrorMessage = "A Lead mező kitöltése kötelező")]
    [StringLength(500, MinimumLength = 20, ErrorMessage = "A Lead hossza nem megfelelő (20-500 karakter)")]
    string Lead,
    [StringLength(5000, MinimumLength = 100, ErrorMessage = "A szekció hossza nem megfelelő (100-5000 karakter) )")]
    [MinLength(100)] [MaxLength(524288)] string FirstSection,
    [StringLength(5000, MinimumLength = 100, ErrorMessage = "A szekció hossza nem megfelelő (100-5000 karakter) )")]
    string LastSection,
    SummaryDto? Summary
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Summary == null) yield break;
        if (string.IsNullOrEmpty(Summary.SecondSection) && !string.IsNullOrEmpty(Summary.ThirdSection))
            yield return new ValidationResult("Nem töltheted ki a harmadik szekciót, amíg a második üres.", [nameof(Summary.SecondSection)]);
                
        if (string.IsNullOrEmpty(Summary.ThirdSection) && !string.IsNullOrEmpty(Summary.FourthSection))
            yield return new ValidationResult("Nem töltheted ki a negyedik szekciót, amíg a harmadik üres.", [nameof(Summary.ThirdSection)]);
    }
}

public record ArticleUpdateDto(
    [Required(ErrorMessage = "Az azonosító kötelező")]
    Guid Id,
    Guid? GrandPrixId,
    [Required(ErrorMessage = "A cím kitöltése kötelező")]
    [StringLength(200, MinimumLength = 5, ErrorMessage = "A cím hossza nem megfelelő (5-200 karakter)")]
    string Title,
    [StringLength(200, MinimumLength = 5, ErrorMessage = "A slug hossza nem megfelelő (5-200 karakter)")]
    string Slug,
    [Required(ErrorMessage = "Kötelező megadni a hír összefoglaló-e?")]
    bool IsReview,
    [Required(ErrorMessage = "A Lead mező kitöltése kötelező")]
    [StringLength(500, MinimumLength = 20, ErrorMessage = "A Lead hossza nem megfelelő (20-500 karakter)")]
    string Lead,
    [StringLength(5000, MinimumLength = 100, ErrorMessage = "A szekció hossza nem megfelelő (100-5000 karakter) )")]
    [MinLength(100)] [MaxLength(524288)] string FirstSection,
    [StringLength(5000, MinimumLength = 100, ErrorMessage = "A szekció hossza nem megfelelő (100-5000 karakter) )")]
    string LastSection,
    SummaryDto? Summary
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Summary == null) yield break;
        if (string.IsNullOrEmpty(Summary.SecondSection) && !string.IsNullOrEmpty(Summary.ThirdSection))
            yield return new ValidationResult("Nem töltheted ki a harmadik szekciót, amíg a második üres.", [nameof(Summary.SecondSection)]);
                
        if (string.IsNullOrEmpty(Summary.ThirdSection) && !string.IsNullOrEmpty(Summary.FourthSection))
            yield return new ValidationResult("Nem töltheted ki a negyedik szekciót, amíg a harmadik üres.", [nameof(Summary.ThirdSection)]);
    }
}

public record ArticleListDto(
    [Required] Guid Id,
    [StringLength(200, MinimumLength = 5)] string Title,
    [StringLength(500, MinimumLength = 20)]
    string Lead,
    [StringLength(200, MinimumLength = 5)] string Slug,
    DateTime DatePublished
);

public record SummaryDto(
    [MinLength(100)] [MaxLength(5000)] string SecondSection,
    [MinLength(100)] [MaxLength(5000)] string ThirdSection,
    [MinLength(100)] [MaxLength(5000)] string FourthSection
);

public record ArticleDetailDto(
    [Required] Guid Id,
    [StringLength(200, MinimumLength = 5)] string Title,
    [StringLength(500, MinimumLength = 20)] string Lead,
    [StringLength(200, MinimumLength = 5)] string Slug,
    [Required] bool IsReview,
    [MinLength(100)] [MaxLength(524288)] string FirstSection,
    [MinLength(100)] [MaxLength(524288)] string LastSection,
    List<string> MiddleSections,
    Guid AuthorId,
    [StringLength(50, MinimumLength = 3)] string AuthorName,
    Guid? GrandPrixId,
    [StringLength(100)] string? GrandPrixName,
    DateTime DatePublished,
    DateTime DateUpdated
);