using System.ComponentModel.DataAnnotations;

namespace DTOs.Polls;

public record PollCreateDto(
    [Required(ErrorMessage = "A Cím megadása kötelező")]
    [StringLength(100, MinimumLength = 10, ErrorMessage = "A leírás 10 és 100 karakter között kell legyen")]
    string Title,
    [Required(ErrorMessage = "A Leírás közelező")]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "A leírás 10 és 500 karakter között kell legyen")]
    string Description,
    string Tag,
    [Required(ErrorMessage = "A lejárat dátumának megadása kötelező")]
    DateTime ExpiresAt,
    [Required(ErrorMessage = "Az opciók megadása kötelező")]
    [MinLength(2, ErrorMessage = "A szavazásnak legalább 2 opciója kell legyen")]
    [MaxLength(10, ErrorMessage = "A szavazásnak maximum 10 opciója lehet")]
    List<string> Options
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ExpiresAt <= DateTime.Now)
            yield return new ValidationResult("A szavazás lejáratátának a jövőben kell lennie", [nameof(ExpiresAt)]);

        if (Options.Any(string.IsNullOrWhiteSpace))
            yield return new ValidationResult("Az opciók nem lehetnek üresek", [nameof(Options)]);

        if (Options.Count != Options.Distinct().Count())
            yield return new ValidationResult("Az opcióknak egyedinek kell lenniük", [nameof(Options)]);

        foreach (var option in Options)
        {
            if (option.Length is < 2 or > 100)
                yield return new ValidationResult("Minden opció 2 és 100 karakter között kell legyen",
                    [nameof(Options)]);
        }
    }
}

public record PollListDto(
    Guid Id,
    string Title,
    string Description,
    string Tag,
    DateTime ExpiresAt
);

public record PollDto(
    Guid Id,
    Guid? AuthorId,
    string AuthorName,
    string Title,
    string Tag,
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