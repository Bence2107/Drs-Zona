using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using Repositories.Interfaces.News;

namespace DTOs.News;

public record ArticleCreateDto(
    Guid? GrandPrixId,
    [Required(ErrorMessage = "Az író megadása kötelező")]
    Guid AuthorId,
    [Required(ErrorMessage = "A cím kitöltése kötelező")]
    [StringLength(200, MinimumLength = 5, ErrorMessage = "A cím hossza nem megfelelő (5-200 karakter)")]
    string Title,
    [SlugExists]
    [StringLength(200, MinimumLength = 5, ErrorMessage = "A slug hossza nem megfelelő (5-200 karakter)")]
    string Slug,
    string Tag,
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
    Guid Id,
    string Title,
    string Lead,
    string Tag,
    bool IsReview,
    string Slug,
    DateTime DatePublished,
    string? PrimaryImageUrl
);

public record SummaryDto(
    [MinLength(100, ErrorMessage = "A szekció hossza minimum 100 karakter")] 
    [MaxLength(5000,  ErrorMessage = "A szekció hossza maximum 5000 karakter")] 
    string SecondSection,
    [MinLength(100, ErrorMessage = "A szekció hossza minimum 100 karakter")] 
    [MaxLength(5000,  ErrorMessage = "A szekció hossza maximum 5000 karakter")] 
    string ThirdSection,
    [MinLength(100, ErrorMessage = "A szekció hossza minimum 100 karakter")] 
    [MaxLength(5000,  ErrorMessage = "A szekció hossza maximum 5000 karakter")] 
    string FourthSection
);

public record ArticleDetailDto(
    Guid Id,
    string Title,
    string Lead,
    string Slug,
    bool IsReview,
    string Tag,
    string FirstSection,
    string LastSection,
    List<string> MiddleSections,
    Guid? AuthorId,
    string AuthorName,
    DateTime DatePublished,
    DateTime DateUpdated,
    string? PrimaryImageUrl,
    string? SecondaryImageUrl,
    string? ThirdImageUrl,
    string? LastImageUrl,
    string? AuthorImageUrl
);

public class SlugExistsAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext ctx)
    {
        // Ha üres, a [Required] attribútum dolga jelezni, itt engedjük tovább
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return ValidationResult.Success;
        }

        var slug = value.ToString()!;
        var repo = ctx.GetRequiredService<IArticlesRepository>();
        
        var existingArticle = repo.GetArticleBySlug(slug).GetAwaiter().GetResult();

        if (existingArticle == null) return ValidationResult.Success;
        if (ctx.ObjectInstance is not ArticleUpdateDto updateDto)
            return new ValidationResult("Ez a slug már létezik. Kérlek módosítsd!");
        
        return existingArticle.Id == updateDto.Id ? ValidationResult.Success : new ValidationResult("Ez a slug már létezik. Kérlek módosítsd!");
    }
}