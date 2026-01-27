using System.ComponentModel.DataAnnotations;

namespace DTOs.News;

public record CommentCreateDto(
    [Required(ErrorMessage = "A cikk azonosítója kötelező")] 
    [Range(1, int.MaxValue, ErrorMessage = "Érvénytelen cikk azonosító")] 
    Guid ArticleId,
    Guid? ReplyToCommentId,
    [Required(ErrorMessage = "A komment tartalma nem lehet üres")]
    [StringLength(1000, MinimumLength = 1, ErrorMessage = "A komment hossza 1 és 1000 karakter között lehet")]
    string Content
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Content))
            yield return new ValidationResult("A komment nem tartalmazhat csak szóközt!", [nameof(Content)]);
    }
}

public record CommentContentUpdateDto(
    [Required(ErrorMessage = "Az azonosító kötelező")]
    Guid Id,
    [Required(ErrorMessage = "A cikk azonosítója kötelező")]
    Guid ArticleId,
    [Required(ErrorMessage = "A tartalom nem lehet üres")]
    [StringLength(1000, MinimumLength = 1, ErrorMessage = "A tartalom hossza 1 és 1000 karakter között lehet")]
    string Content
);

public record CommentUpdateVoteDto(
    Guid Id,
    Guid ArticleId,
    int UpVotes,
    int DownVotes
);

public record CommentDetailDto(
    Guid Id,
    Guid UserId,
    Guid? ReplyToCommentId,
    string Username,
    string Content,
    int UpVotes,
    int DownVotes,
    DateTime DateCreated,
    DateTime DateUpdated
);