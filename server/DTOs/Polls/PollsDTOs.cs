using System.ComponentModel.DataAnnotations;

namespace DTOs.Polls;

public record PollCreateDto(
    [Required(ErrorMessage = "A Cím közelező")]
    [StringLength(100, MinimumLength = 10, ErrorMessage = "A leírás 10 és 100 karakter között kell legyen")]
    string Title,
    [Required(ErrorMessage = "A Leírás közelező")]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "A leírás 10 és 500 karakter között kell legyen")]
    string Description,
    [Required(ErrorMessage = "A lejárat dátuma kötelező")]
    DateTime ExpiresAt,
    [Required(ErrorMessage = "Az opciók megadása kötelező")]
    [MinLength(2, ErrorMessage = "Poll must have at least 2 options")]
    [MaxLength(10, ErrorMessage = "Poll cannot have more than 10 options")]
    List<string> Options
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ExpiresAt <= DateTime.Now)
            yield return new ValidationResult("Expiration date must be in the future", [nameof(ExpiresAt)]);

        if (Options.Any(string.IsNullOrWhiteSpace))
            yield return new ValidationResult("Poll options cannot be empty", [nameof(Options)]);

        if (Options.Count != Options.Distinct().Count())
            yield return new ValidationResult("Poll options must be unique", [nameof(Options)]);

        foreach (var option in Options)
        {
            if (option.Length is < 2 or > 100)
                yield return new ValidationResult("Each option must be between 2 and 100 characters",
                    [nameof(Options)]);
        }
    }
}

public record PollListDto(
    Guid Id,
    string Title
);

public record PollDto(
    Guid Id,
    Guid AuthorId,
    string AuthorName,
    string Description,
    DateTime CreatedAt,
    DateTime ExpiresAt,
    bool IsActive,
    List<PollOptionDto> PollOptions,
    bool IsExpired,
    int TotalVotes,
    Guid? UserVoteOptionId
){
    public bool HasVoted => UserVoteOptionId.HasValue;
}

public record PollOptionDto(
    Guid Id,
    string Text,
    int VoteCount,
    double VotePercentage,
    bool IsUserChoice
);