using System.ComponentModel.DataAnnotations;

namespace DTOs.News;

public record ArticleCreateDto(
    [Range(1, int.MaxValue, ErrorMessage = "Érvénytelen Nagydíj azonosító")] int? GrandPrixId,
    [Required(ErrorMessage = "A cím kitöltése kötelező")]
    [StringLength(200, MinimumLength = 5, ErrorMessage = "A cím hossza nem megfelelő (5-200 karakter)")]
    string Title,
    [Required(ErrorMessage = "A Lead mező kitöltése kötelező")]
    [StringLength(500, MinimumLength = 20, ErrorMessage = "A Lead hossza nem megfelelő (20-500 karakter)")]
    string Lead,
    [Required(ErrorMessage = "A tartalom mező kitöltése kötelező")]
    [StringLength(524288, MinimumLength = 100, ErrorMessage = "A tartalom túl rövid (min 100 karakter)")]
    string Content
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrEmpty(Lead) && !string.IsNullOrEmpty(Content) && Lead.Length > Content.Length)
            yield return new ValidationResult("A Lead nem lehet hosszabb a cikk tartamánál", [nameof(Lead)]);
    }
}

public record ArticleUpdateDto(
    [Required(ErrorMessage = "Az azonosító kötelező")]
    [Range(1, int.MaxValue)] int Id,
    [Range(1, int.MaxValue, ErrorMessage = "Érvénytelen Nagydíj azonosító")] int? GrandPrixId,
    [Required(ErrorMessage = "A cím kitöltése kötelező")]
    [StringLength(200, MinimumLength = 5, ErrorMessage = "A cím hossza nem megfelelő (5-200 karakter)")]
    string Title,
    [Required(ErrorMessage = "A Lead mező kitöltése kötelező")]
    [StringLength(500, MinimumLength = 20, ErrorMessage = "A Lead hossza nem megfelelő (20-500 karakter)")]
    string Lead,
    [Required(ErrorMessage = "A tartalom mező kitöltése kötelező")]
    [StringLength(524288, MinimumLength = 100, ErrorMessage = "A tartalom túl rövid (min 100 karakter)")]
    string Content
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrEmpty(Lead) && !string.IsNullOrEmpty(Content) && Lead.Length > Content.Length)
            yield return new ValidationResult("A Lead nem lehet hosszabb a cikk tartamánál", [nameof(Lead)]);
    }
}

public record ArticleListDto(
    [Range(1, int.MaxValue)] int Id,
    [StringLength(200, MinimumLength = 5)] string Title,
    [StringLength(500, MinimumLength = 20)]
    string Lead,
    DateTime DatePublished
);

public record ArticleDetailDto(
    [Range(1, int.MaxValue)] int Id,
    [StringLength(200, MinimumLength = 5)] string Title,
    [StringLength(500, MinimumLength = 20)] string Lead,
    [MinLength(100)] [MaxLength(524288)] string Content,
    [Range(1, int.MaxValue)] int AuthorId,
    [StringLength(50, MinimumLength = 3)] string AuthorName,
    [Range(1, int.MaxValue)] int? GrandPrixId,
    [StringLength(100)] string? GrandPrixName,
    DateTime DatePublished,
    DateTime DateUpdated
);

public record ArticleRecentDto(
    [Range(1, int.MaxValue)] int Id,
    [StringLength(200, MinimumLength = 5)] string Title,
    [StringLength(500, MinimumLength = 20)]
    string Lead,
    [StringLength(50, MinimumLength = 3)] string AuthorName,
    DateTime DatePublished
);